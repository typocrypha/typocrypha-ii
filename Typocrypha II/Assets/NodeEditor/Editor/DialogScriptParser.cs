﻿using System.Collections;
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
    Use '\' to escape control sequences 
        (e.g., to have a literal newline in dialog, put '\' at the end of the line) 
    To have a literal backslash, you need to use '\\' 

******************* Format *******************
    SCRIPT := [STATEMENT]
    
    % Note the required newline at the end of a statement
    STATEMENT := {VIEW_SWITCH, DIALOG, COMMENT, NODE} \n
    
    % Switches the dialog view mode
    VIEW_SWITCH := +VIEW_LABEL
    VIEW_LABEL := {"vn", "chat", "an", "bubble"}

    DIALOG := *speaker name (can be empty)* (*expression*) : *line of dialog*

    DIALOG := *speaker name (can be empty)* : *line of dialog*

    COMMENT := // *comment*

    % Allows for specifying various nodes
    NODE := >NODE_LABEL, [NODE_ARGUMENT_LIST]

    NODE_LABEL := {"addchar"}

    NODE_ARGUMENT_LIST := *arg 1*,*arg 2*, ... , *arg n*

******************* Node Types *******************
    >addchar, *character name*, *x position*, *y position*
        -Adds a new character to the scene with default pose and expression.
    >removechar, *character name*
        -Removes character.
*/

/// <summary>
/// Parses text scripts into dialog node canvases
/// </summary>
public class DialogScriptParser : EditorWindow
{
    TextAsset textScript; // Text script asset
    NodeCanvas canvas; // Generated canvas
    System.Type currView = typeof(DialogViewVN); // Current dialog view
    float pos; // Position of current node

    AssetBundle characterDataBundle; // Character data bundle
    CharacterData[] allCharacterData; // All loaded character data

    readonly char[] lineDelim = new char[] { '\n' }; // Line delimiter.
    readonly char[] argDelim = new char[] { ',' }; // List of arguments delimiter.
    readonly char[] nameMarker = new char[] { ':' }; // Speaker's name delim.
    readonly char[] nodeMarker = new char[] { '>' }; // Node marker.
    readonly char[] viewSwitchMarker = new char[] { '+' }; // View switching marker.
    readonly char[] escape = new char[] { '\\' }; // Escape character.

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
        {"removechar", typeof(RemoveCharacter) },
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
        // Parse text into node canvas.
        Parse();
        // Save canvas.
        canvas.saveName = "Assets/ScriptableObjects/DialogScenes/" + canvas.name + ".asset";
        NodeEditorSaveManager.SaveNodeCanvas(canvas.saveName, ref canvas, true);
        AssetBundle.UnloadAllAssetBundles(true);
    }

    // Parses text into node canvas
    void Parse()
    {
        string text = textScript.text;
        text = Regex.Replace(text, "/{2}.*?\n", "\n"); // Remove comments
        text = Regex.Replace(text, "/[\x2A].*?[\x2A]/", "", RegexOptions.Singleline); // Remove block comments
        text = Regex.Replace(text, "\r", ""); // Remove carriage returns
        pos = 0f; // Position of current node
        Node prev = null; // Previous node (init as start node).
        prev = CreateNode(GameflowStartNode.ID) as GameflowStartNode;

        text = Regex.Replace(text,":\\s*\n",":"); // Remove newlines after character name delimiters.
        string[] lines = text.Split(lineDelim, escape); // Separate lines
        for (int i = 0; i < lines.Length; i++)
        {
            Debug.Log("parsing:" + lines[i]);
            try
            {
                ParseLine(lines[i], ref prev);
            }
            catch
            {
                Debug.LogError("Line " + (i+1) + ": Script parsing error");
            }
        }
        // Create end node.
        var endNode = Node.Create(EndAndHide.ID, Vector2.right * pos) as EndAndHide;
        (prev as BaseNodeIO).toNextOUT.TryApplyConnection(endNode.fromPreviousIN);
    }

    // Parses a single line
    void ParseLine(string line, ref Node prev)
    {
        line = line.Trim();
        if (line.Length < 2) return; // Empty line.
        List<Node> nodes = null; // Constructed nodes.
        if (viewSwitchMarker.Contains(line[0])) // View switch.
        {
            string[] dialogLine = line.Split(viewSwitchMarker);
            currView = viewMap[dialogLine[1]];
        }
        else if (nodeMarker.Contains(line[0])) // General node.
        {
            nodes = ParseGeneralNode(line); // Parse node.
        }
        else // Line of dialog.
        {
            nodes = ParseDialogNode(line); // Parse dialog
        }
        if (nodes != null)
        {
            foreach(var node in nodes)
            {
                // Connect to previous
                (prev as BaseNodeOUT).toNextOUT.TryApplyConnection((node as BaseNodeIO).fromPreviousIN, true);
                prev = node;
            }
        }
    }

    // Parses a general node (not dialog) line. Returns constructed nodes.
    List<Node> ParseGeneralNode(string line)
    {
        string[] dialogLine = line.Split(nodeMarker, escape);
        string[] args = dialogLine[1].Split(argDelim, escape);
        List<Node> nodes = new List<Node>();
        System.Type nodeType = nodeMap[args[0]];
        if (nodeType == typeof(AddCharacter))
        {
            var gnode = CreateNode(AddCharacter.ID) as AddCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            gnode.targetPos = new Vector2(float.Parse(args[2]), float.Parse(args[3]));
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(RemoveCharacter))
        {
            var gnode = CreateNode(RemoveCharacter.ID) as RemoveCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            nodes.Add(gnode);
        }
        return nodes;
    }

    // Parses a single dialog line. Returns constructed nodes.
    List<Node> ParseDialogNode(string line)
    {
        string[] dialogLine = line.Split(nameMarker, escape);
        List<Node> nodes = new List<Node>();
        string charName;
        if (dialogLine[0].Contains('('))
        {
            charName = dialogLine[0].Substring(0, dialogLine[0].IndexOf('(')).Trim();
            string expr = Regex.Match(dialogLine[0], @"\(([^\)]*)\)").Value;
            expr = expr.Substring(1, expr.Length - 2);
            var gnode = CreateNode(SetExpression.ID) as SetExpression;
            gnode.characterData = GetCharacterData(charName);
            gnode.expr = expr;
            nodes.Add(gnode);
        }
        else
        {
            charName = dialogLine[0].Trim();
            string expr = "normal"; // Normal default expression.
            var gnode = CreateNode(SetExpression.ID) as SetExpression;
            gnode.characterData = GetCharacterData(charName);
            gnode.expr = expr;
            nodes.Add(gnode);
        }
        DialogNode dnode = null; // Node for dialog.
        if (currView == typeof(DialogNodeVN))
        {
            dnode = CreateNode(DialogNodeVN.ID) as DialogNodeVN;
        }
        else if (currView == typeof(DialogNodeChat))
        {
            dnode = CreateNode(DialogNodeChat.ID) as DialogNodeChat;
        }
        else if (currView == typeof(DialogNodeAN))
        {
            dnode = CreateNode(DialogNodeAN.ID) as DialogNodeAN;
        }
        else if (currView == typeof(DialogNodeBubble))
        {
            dnode = CreateNode(DialogNodeBubble.ID) as DialogNodeBubble;
        }
        dnode.characterName = charName;
        dnode.text = dialogLine[1].Trim();
        nodes.Add(dnode);
        return nodes;
    }

    // Get character data from alias.
    CharacterData GetCharacterData(string alias)
    {
        var cdata = allCharacterData.Single(c => c.aliases.Contains(alias)); // Find character data.
        return AssetDatabase.LoadAssetAtPath<CharacterData>("Assets/ScriptableObjects/CharacterData/" + cdata.name + ".asset");
    }

    // Creates node and shifts position.
    Node CreateNode(string id)
    {
        var gnode = Node.Create(id, Vector2.right * pos, canvas);
        pos += gnode.MinSize.x + nodeSpacing;
        return gnode;
    }
}