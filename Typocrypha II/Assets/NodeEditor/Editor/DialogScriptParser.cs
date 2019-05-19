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
    [ ITEM ] -> Zero or more copies of item
    { ITEM1, ITEM2, ... } -> Any single one of the contained items
    % comment -> Comment
    "label" -> String literal (e.g. view types)
    *descriptor* -> Variable text (e.g. dialog)
     
    Newline characters are used as statement delimiters
    Use '\' to escape control sequences (NOT IMPLEMENTED YET)
        (e.g., to have a literal newline in dialog, put '\' at the end of the line) (NOT IMPLEMENTED YET)
    To have a literal backslash, you need to use '\\' (NOT IMPLEMENTED YET)

******************* Format *******************
    SCRIPT := [STATEMENT]
    
    % Note the required newline at the end of a statement
    STATEMENT := {VIEW_SWITCH, DIALOG, COMMENT, NODE} \n
    
    % Switches the dialog view mode
    VIEW_SWITCH := +VIEW_LABEL
    VIEW_LABEL := {"vn", "chat", "an", "bubble"}

    DIALOG := *speaker name (can be empty)* : *line of dialog*

    COMMENT := // *comment*

    % Allows for specifying various nodes
    NODE := >NODE_LABEL, [NODE_ARGUMENT_LIST]

    NODE_LABEL := {"addchar", "end"}

    NODE_ARGUMENT_LIST := *arg 1*,*arg 2*, ... , *arg n*

******************* Node Types *******************
    >addchar, *character name*, *x position*, *y position*
        -Adds a new character to the scene with default pose and expression.
*/

/// <summary>
/// Parses text scripts into dialog node canvases
/// </summary>
public class DialogScriptParser : EditorWindow
{
    TextAsset textScript; // Text script asset
    NodeCanvas canvas; // Generated canvas
    System.Type currView; // Current dialog view

    AssetBundle characterDataBundle; // Character data bundle
    CharacterData[] allCharacterData; // All loaded character data
    
    char[] lineDelim = new char[] { '\n' }; // Line delimiter.
    char[] argDelim = new char[] { ',' }; // List of arguments delimiter.
    char[] nameMarker = new char[] { ':' }; // Speaker's name delim.
    char[] nodeMarker = new char[] { '>' }; // Node marker.
    char[] viewSwitchMarker = new char[] { '+' }; // View switching marker.
    char[] escape = new char[] { '\\' }; // Escape character.
    string commentRegex = "/{2}"; // Comment regex "//".

    // Dialog view labels.
    Dictionary<string, System.Type> viewMap = new Dictionary<string, System.Type>
    {
        {"vn", typeof(DialogNodeVN) },
        {"chat", typeof(DialogNodeChat) },
        {"an", typeof(DialogNodeAN) },
        {"bubble", typeof(DialogNodeBubble) },
    };

    // General node labels.
    Dictionary<string, System.Type> nodeMap = new Dictionary<string, System.Type>
    {
        {"addchar", typeof(AddCharacter) },
        {"end", typeof(GameflowEndNode) },
    };

    const float nodeSpacing = 40f;

    [MenuItem("Window/DialogScriptParser")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogScriptParser));
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
        AssetBundle.UnloadAllAssetBundles(true);
        characterDataBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "characterdata"));
        allCharacterData = characterDataBundle.LoadAllAssets<CharacterData>();
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
        AssetBundle.UnloadAllAssetBundles(true);
    }

    // Parses text into node canvas
    void Parse()
    {
        string text = textScript.text;
        text = Regex.Replace(text, commentRegex + ".*?\n", "\n"); // Remove comments
        float pos = 0f; // Position of current node
        Node prev = null; // Previous node (init as start node).
        prev = Node.Create(GameflowStartNode.ID, Vector2.right * pos) as GameflowStartNode;
        pos += prev.MinSize.x + nodeSpacing;

        // PARSE LINEBREAKS '\' AND ESCAPES
        string[] lines = text.Split(lineDelim); // Separate lines
        for (int i = 0; i < lines.Length; i++)
        {
            ParseLine(lines[i], ref pos, ref prev);
        }
        // Create end node.
        var endNode = Node.Create(EndAndHide.ID, Vector2.right * pos) as EndAndHide;
        (prev as BaseNodeIO).toNextOUT.TryApplyConnection(endNode.fromPreviousIN);
    }

    // Parses a single line
    void ParseLine(string line, ref float pos, ref Node prev)
    {
        line = line.Trim();
        if (line.Length < 2) return; // Empty line.
        else if (viewSwitchMarker.Contains(line[0])) // View switch.
        {
            string[] dialogLine = line.Split(viewSwitchMarker);
            currView = viewMap[dialogLine[1]];
        }
        else if (nodeMarker.Contains(line[0])) // General node.
        {
            string[] dialogLine = line.Split(nodeMarker);
            string[] nodeArgs = dialogLine[1].Split(argDelim);
            var node = ParseGeneralNode(nodeArgs, pos); // Parse node.
            pos += node.MinSize.x + nodeSpacing; // Update position
            // Connect to previous
            (prev as BaseNodeOUT).toNextOUT.TryApplyConnection((node as BaseNodeIO).fromPreviousIN, true);
            prev = node;
        }
        else // Line of dialog.
        {
            var node = ParseDialogNode(line, pos); // Parse dialog
            pos += node.MinSize.x + nodeSpacing; // Update position
            // Connect to previous
            (prev as BaseNodeOUT).toNextOUT.TryApplyConnection((node as BaseNodeIO).fromPreviousIN, true);
            prev = node;   
        }
    }

    // Parses a general node (not dialog) line. Returns constructed node.
    Node ParseGeneralNode(string[] args, float pos)
    {
        Node node = null;
        System.Type nodeType = nodeMap[args[0]];
        if (nodeType == typeof(AddCharacter))
        {
            var gnode = Node.Create(AddCharacter.ID, Vector2.right * pos) as AddCharacter;
            var cdata = allCharacterData.Single(c => c.aliases.Contains(args[1])); // Find character data.
            gnode.characterData = AssetDatabase.LoadAssetAtPath<CharacterData>("Assets/ScriptableObjects/CharacterData/" + cdata.name + ".asset");
            gnode.targetPos = new Vector2(float.Parse(args[2]), float.Parse(args[3]));
            node = gnode;
        }
        return node;
    }

    // Parses a single dialog line. Returns constructed node.
    Node ParseDialogNode(string line, float pos)
    {
        string[] dialogLine = line.Split(nameMarker);
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
