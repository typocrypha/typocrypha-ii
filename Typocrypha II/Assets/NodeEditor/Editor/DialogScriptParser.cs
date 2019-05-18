using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using NodeEditorFramework;
using Gameflow;

/*
******************* Metasyntax *******************
    CAPITAL_LETTERS -> Type of parsable item
    := -> Defined as
    [ ITEM ] -> Several copies of item
    { ITEM1, ITEM2, ... } -> Any single one of the contained items
    \ -> Escape character
    % comment -> Comment
    "label" -> String literal (e.g. view types)
    *descriptor* -> Any text (e.g. dialog)
     
    White space is ignored
 
******************* Format *******************
    SCRIPT := [STATEMENT]
    
    % Note the required newline at the end of a statement
    STATEMENT := {VIEW_SWITCH, DIALOG, COMMENT} \n
    
    % Switches the dialog view mode
    VIEW_SWITCH := +VIEW_LABEL
    VIEW_LABEL := {"vn", "chat", "an", "bubble"}

    DIALOG := *speaker name (can be empty)* : *line of dialog*

    COMMENT := // *comment*
*/

/// <summary>
/// Parses text scripts into dialog node canvases
/// </summary>
public class DialogScriptParser : EditorWindow
{
    TextAsset textScript; // Text script asset
    NodeCanvas canvas; // Generated canvas
    System.Type currView; // Current dialog view
    
    char[] lineDelim = new char[] { '\n' }; // Line delimiter.
    char[] nameDelim = new char[] { ':' }; // Speaker's name delim.
    string commentRegex = "/{2}"; // Comment regex "//".
    char[] viewSwitch = new char[] { '+' }; // View switching marker.
    char[] escape = new char[] { '\\' }; // Escape character.

    Dictionary<string, System.Type> viewMap = new Dictionary<string, System.Type>
    {
        {"vn", typeof(DialogNodeVN) },
        {"chat", typeof(DialogNodeChat) },
        {"an", typeof(DialogNodeAN) },
        {"bubble", typeof(DialogNodeBubble) },
    };

    const float nodeSpacing = 40f;

    [MenuItem("Window/DialogScriptParser")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(DialogScriptParser));
    }

    void OnGUI()
    {
        GUILayout.BeginVertical("Box");

        GUILayout.BeginHorizontal();
        GUILayout.Label("Text Script");
        textScript = EditorGUILayout.ObjectField(textScript, typeof(TextAsset), false) as TextAsset;
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Node Canvas");
        canvas = EditorGUILayout.ObjectField(canvas, typeof(NodeCanvas), false) as NodeCanvas;
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        if (textScript != null && GUILayout.Button("Generate Canvas", GUILayout.Width(120f)))
        {
            GenerateCanvas();
        }
        
    }

    // Generates node canvas from script
    void GenerateCanvas()
    {
        if (canvas == null) // Generate new canvas if empty
        {
            canvas = NodeCanvas.CreateCanvas(typeof(DialogCanvas));
            canvas.name = textScript.name;
        }
        else // Otherwise overwrite
        {
            canvas.nodes.Clear();
        }

        Parse();

        canvas.saveName = "Assets/ScriptableObjects/DialogScenes/" + canvas.name + ".asset";
        NodeEditorSaveManager.SaveNodeCanvas(canvas.saveName, ref canvas, true);
    }

    // Parses text into node canvas
    void Parse()
    {
        string text = textScript.text;
        text = Regex.Replace(text, commentRegex + ".*?[\n]", "\n"); // Remove comments
        string[] lines = text.Split(lineDelim);
        float pos = 0f; // Position of current node
        Node prev = null; // Previous node
        for (int i = 0; i < lines.Length; i++)
        {
            ParseLine(lines[i], ref pos, ref prev);
        }
    }

    // Parses a single line
    void ParseLine(string line, ref float pos, ref Node prev)
    {
        line = line.Trim();
        if (line.Length < 2) return; // Empty line.
        else if (viewSwitch.Contains(line[0])) // View switch.
        {
            string[] dialogLine = line.Split(viewSwitch);
            currView = viewMap[dialogLine[1]];
        }
        else // Line of dialog.
        {
            var node = ParseDialogNode(line, pos); // Parse dialog
            pos += node.MinSize.x + nodeSpacing; // Update position
            if (prev != null) // Connect to previous
                (prev as BaseNodeIO).toNextOUT.TryApplyConnection((node as BaseNodeIO).fromPreviousIN, true);
            prev = node;   
        }
    }

    // Parses a single dialog line. Returns constructed node.
    Node ParseDialogNode(string line, float pos)
    {
        string[] dialogLine = line.Split(nameDelim);
        DialogNode node = null;
        if (currView == typeof(DialogNodeVN))
        {
            node = Node.Create(DialogNodeVN.ID, Vector2.right * pos, canvas) as DialogNodeVN;
        }
        else if (currView == typeof(DialogNodeChat))
        {
            node = Node.Create(DialogNodeChat.ID, Vector2.right * pos, canvas) as DialogNodeChat;
        }
        else if (currView == typeof(DialogNodeAN))
        {
            node = Node.Create(DialogNodeAN.ID, Vector2.right * pos, canvas) as DialogNodeAN;
        }
        else if (currView == typeof(DialogNodeBubble))
        {
            node = Node.Create(DialogNodeBubble.ID, Vector2.right * pos, canvas) as DialogNodeBubble;
        }
        node.characterName = dialogLine[0].Trim();
        node.text = dialogLine[1].Trim();
        return node;
    }
}
