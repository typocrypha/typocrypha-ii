using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using NodeEditorFramework;
using Gameflow;

// Syntax: https://docs.google.com/document/d/1za4Xt3NwA6cOirZnnqCrD0kbA193A7Ypk6n_yVwa1e4/edit#heading=h.jg4trxckxznj

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
    readonly char[] nameMarker = new char[] { ':' }; // Speaker's name marker for dialog lines.
    readonly char[] nodeMarker = new char[] { '>' }; // Node marker.
    readonly char[] viewSwitchMarker = new char[] { '+' }; // View switching marker.
    readonly char[] exprMarker = new char[] { '(', ')' }; // Speaker's expression marker for dialog lines.
    readonly string exprPat = @"\(([^\)]*)\)"; // Expression marker pattern.
    readonly char[] poseMarker = new char[] { '[', ']' }; // Speaker's pose marker for dialog lines.
    readonly string posePat = @"\[([^\)]*)\]"; // Pose marker pattern.
    readonly string displayPat = "\".*?\""; // Display name marker pattern.
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
        {"playbgm", typeof(PlayBgm) },
        {"stopbgm", typeof(StopBgm) },
        {"setbg", typeof(SetBackgroundNode) },
        {"end", typeof(GameflowEndNode) },
    };

    AnimationCurve bgmFadeIn = new AnimationCurve(); // Default fade in curve
    AnimationCurve bgmFadeOut = new AnimationCurve(); // Default fade out curve

    const float nodeSpacing = 40f;

    [MenuItem("Window/DialogScriptParser")]
    public static void ShowWindow()
    {
        GetWindow(typeof(DialogScriptParser));
    }

    void OnGUI()
    {
        // Main asset fields
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
        // Custom settings
        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal("Box");
        bgmFadeIn = EditorGUILayout.CurveField("BGM Fade In Curve", bgmFadeIn, GUILayout.Height(50f));
        bgmFadeOut = EditorGUILayout.CurveField("BGM Fade Out Curve", bgmFadeOut, GUILayout.Height(50f));
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        // Generate button
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
        #region Preprocessing text
        text = Regex.Replace(text, "/{2}.*?\n", "\n"); // Remove comments
        text = Regex.Replace(text, "/[\x2A].*?[\x2A]/", "", RegexOptions.Singleline); // Remove block comments
        text = Regex.Replace(text, "\r", ""); // Remove carriage returns
        text = Regex.Replace(text, ":\\s*\n", ":"); // Remove newlines after character name delimiters.
        #endregion
        string[] lines = text.Split(lineDelim, escape); // Separate lines

        pos = 0f; // Position of current node
        Node prev = null; // Previous node (init as start node).
        prev = CreateNode(GameflowStartNode.ID) as GameflowStartNode;
        currView = viewMap["vn"]; // Default to visual novel view
        for (int i = 0; i < lines.Length; i++)
        {
            //Debug.Log("parsing:" + lines[i]);
            try
            {
                ParseLine(lines[i], ref prev);
            }
            catch
            {
                Debug.LogError("Line " + (i + 1) + ": Script parsing error");
                Debug.LogError("Line " + (i + 1) + ": " + lines[i]);
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
            string[] dialogLine = line.Split(viewSwitchMarker, escape);
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
        else if (nodeType == typeof(PlayBgm))
        {
            var gnode = CreateNode(PlayBgm.ID) as PlayBgm;
            string path = AssetDatabase.FindAssets(args[1], AssetDatabase.GetSubFolders("Assets/Audio/Clips/Music"))[0];
            path = AssetDatabase.GUIDToAssetPath(path);
            gnode.bgm = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            gnode.fadeCurve = bgmFadeIn;
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(StopBgm))
        {
            var gnode = CreateNode(StopBgm.ID) as StopBgm;
            gnode.fadeCurve = bgmFadeOut;
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(SetBackgroundNode))
        {
            var gnode = CreateNode(SetBackgroundNode.ID) as SetBackgroundNode;
            gnode.bgType = (args[1] == "sprite") ? SetBackgroundNode.BgType.Sprite 
                                                 : SetBackgroundNode.BgType.Prefab;
            string path = (args[1] == "sprite") ? AssetDatabase.FindAssets(args[2], AssetDatabase.GetSubFolders("Assets/Graphics/Sprites/Backgrounds"))[0] 
                                                : AssetDatabase.FindAssets(args[2], AssetDatabase.GetSubFolders("Assets/Prefabs/Backgrounds"))[0];
            path = AssetDatabase.GUIDToAssetPath(path);
            if (args[1] == "sprite") gnode.bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            else                     gnode.bgPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            nodes.Add(gnode);
        }
        return nodes;
    }

    // Parses a single dialog line. Returns constructed nodes.
    List<Node> ParseDialogNode(string line)
    {
        string[] dialogLine = line.Split(nameMarker, escape);
        List<Node> nodes = new List<Node>();
        string charName = (dialogLine[0].Contains(exprMarker[0]) || dialogLine[0].Contains(poseMarker[0])) 
                        ? dialogLine[0].Substring(0, dialogLine[0].IndexOfAny(exprMarker.Concat(poseMarker).ToArray())).Trim()
                        : dialogLine[0].Trim();
        if (charName.Contains('"')) charName = charName.Substring(0, charName.IndexOf('"')).Trim();
        string displayName = Regex.Match(dialogLine[0], displayPat).Value; // Displayed speaker name.
        displayName = (displayName.Length > 2) 
                    ? displayName.Substring(1, displayName.Length - 2) 
                    : "";
        CharacterData charData = GetCharacterData(charName); // Character data.
        #region Expression and pose
        string expr = ""; // Expression string (value within parentheticals)
        string pose = ""; // Pose string [value within square brackets]
        if (charData != null)
        {
            if (dialogLine[0].Contains(poseMarker[0])) // Pose
            {
                pose = Regex.Match(dialogLine[0], posePat).Value;
                pose = pose.Substring(1, pose.Length - 2);
            }
            else
            {
                pose = "base"; // Default pose.
            }
            if (dialogLine[0].Contains(exprMarker[0])) // Expression
            {
                expr = Regex.Match(dialogLine[0], exprPat).Value;
                expr = expr.Substring(1, expr.Length - 2);
            }
            else
            {
                expr = "normal"; // Default expression.
            }
        }
        #endregion
        DialogNode dnode = null; // Node for dialog.
        if (currView == typeof(DialogNodeVN))
        {
            // Create expression and pose nodes
            if (charData != null)
            {
                var hnode = CreateNode(SetPose.ID) as SetPose;
                hnode.characterData = charData;
                hnode.pose = pose;
                nodes.Add(hnode);
                var gnode = CreateNode(SetExpression.ID) as SetExpression;
                gnode.characterData = charData;
                gnode.expr = expr;
                nodes.Add(gnode);
            }
            // Create dialog node
            dnode = CreateNode(DialogNodeVN.ID) as DialogNodeVN;
        }
        else if (currView == typeof(DialogNodeChat))
        {
            // USE EXPRESSION TO CHANGE ICON (currently only 1 icon)
            dnode = CreateNode(DialogNodeChat.ID) as DialogNodeChat;
            if (pose == "left")
            {
                (dnode as DialogNodeChat).leftIcon = charData.chat_icon;
            }
            else if (pose == "right")
            {
                (dnode as DialogNodeChat).rightIcon = charData.chat_icon;
            }
            else if (pose == "both")
            {
                (dnode as DialogNodeChat).leftIcon = charData.chat_icon;
                (dnode as DialogNodeChat).rightIcon = charData.chat_icon;
            }
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
        dnode.displayName = displayName;
        dnode.text = dialogLine[1].Trim();
        nodes.Add(dnode);
        return nodes;
    }

    // Get character data asset from alias (null if character doesn't exist).
    CharacterData GetCharacterData(string alias)
    {
        if (!allCharacterData.Any(c => c.aliases.Contains(alias))) return null;
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
