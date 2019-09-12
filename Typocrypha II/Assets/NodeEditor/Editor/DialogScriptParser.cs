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
    public const string assetPath = "Assets/ScriptableObjects/DialogScenes/";
    TextAsset textScript; // Text script asset
    NodeCanvas canvas; // Generated canvas
    System.Type currView = typeof(DialogViewVN); // Current dialog view
    float pos; // Position of current node
    Node prev;

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
    readonly string displayPat = "[\"“].*?[\"”]"; // Display name marker pattern.
    readonly char[] choiceMarker = new char[] { '<', '>' }; // Choice marker for dialog input lines.
    readonly string choicePat = @"\<([^\)]*)\>"; // Input choice prompt marker pattern.
    readonly char[] escape = new char[] { '\\' }; // Escape character.
    readonly char[] displayNameChars = new char[] { '"', '“', '”' }; // Characters that could delimit a display name.
    readonly string googleCommentPat = @"\[\w\]";

    // Dialog view labels.
    Dictionary<string, System.Type> viewMap = new Dictionary<string, System.Type>
    {
        {"vn", typeof(DialogNodeVN) },
        {"chat", typeof(DialogNodeChat) },
        {"an", typeof(DialogNodeAN) },
        {"bubble", typeof(DialogNodeBubble) },
        {"input", typeof(DialogNodeInput) }
    };

    // General node labels.
    Dictionary<string, System.Type> nodeMap = new Dictionary<string, System.Type>
    {
        {"addchar", typeof(AddCharacter) },
        {"removechar", typeof(RemoveCharacter) },
        {"playbgm", typeof(PlayBgm) },
        {"stopbgm", typeof(StopBgm) },
        {"setbg", typeof(SetBackgroundNode) },
        {"fade", typeof(FadeNode) },
        {"end", typeof(GameflowEndNode) },
        {"start" , typeof(GameflowStartNode)},
        {"endt", typeof(EndAndTransition) }
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

    // Generates node canvases from script
    void GenerateCanvas()
    {
        AssetBundle.UnloadAllAssetBundles(true);
        characterDataBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(Application.streamingAssetsPath, "characterdata"));
        allCharacterData = characterDataBundle.LoadAllAssets<CharacterData>();
        canvas = null;
        Parse();
        AssetBundle.UnloadAllAssetBundles(true);
    }

    /// <summary>
    /// Attempts to make a new canvas with the given name, or finds the existing canvas.
    /// Sets the canvas to the 'canvas' field.
    /// </summary>
    /// <param name="canvasName"></param>
    void StartCanvas(string canvasName)
    {
        canvas = AssetDatabase.LoadAssetAtPath<DialogCanvas>(assetPath + canvasName + ".asset");
        if (canvas == null) // Generate new canvas if empty
        {
            canvas = NodeCanvas.CreateCanvas(typeof(DialogCanvas));
            canvas.name = canvasName;
            canvas.Validate();
        }
        else // Otherwise overwrite
        {
            canvas.nodes.Clear();
            canvas.Validate();
        }
        pos = 0f; // Position of current node
        prev = CreateNode(GameflowStartNode.ID) as GameflowStartNode;
        currView = viewMap["vn"]; // Default to visual novel view
    }

    /// <summary>
    /// Saves current canvas.
    /// </summary>
    void EndCanvas()
    {
        canvas.saveName = assetPath + canvas.name + ".asset";
        NodeEditorSaveManager.SaveNodeCanvas(canvas.saveName, ref canvas, true);
    }

    // Parses text into node canvas
    void Parse()
    {
        string text = textScript.text;
        #region Preprocessing text
        text = Regex.Replace(text, googleCommentPat, "");
        text = Regex.Replace(text, "/{2}.*?\n", "\n"); // Remove comments
        text = Regex.Replace(text, "/[\x2A].*?[\x2A]/", "", RegexOptions.Singleline); // Remove block comments
        text = Regex.Replace(text, "\r", ""); // Remove carriage returns
        text = Regex.Replace(text, ":\\s*\n", ":"); // Remove newlines after character name delimiters.
        #endregion
        string[] lines = text.Split(lineDelim, escape); // Separate lines

        for (int i = 0; i < lines.Length; i++)
        {
            //Debug.Log("parsing:" + (i+1) +":" + lines[i]);
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
        // Create end node (if not already there).
        if (!(prev is GameflowEndNode))
        {
            var endNode = CreateNode(EndAndHide.ID) as EndAndHide;
            (prev as BaseNodeIO).toNextOUT.TryApplyConnection(endNode.fromPreviousIN, true);
        }
        EndCanvas();
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
                if (node is BaseNodeIO)
                {
                    (prev as BaseNodeOUT).toNextOUT.TryApplyConnection((node as BaseNodeIO).fromPreviousIN, true);
                }
                else
                {
                    (prev as BaseNodeOUT).toNextOUT.TryApplyConnection((node as GameflowEndNode).fromPreviousIN, true);
                }
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
        if (nodeType == typeof(GameflowStartNode))
        {
            if (canvas != null) EndCanvas(); // End previous canvas.
            StartCanvas(args[1]); // Start new canvas.
        }
        else if (nodeType == typeof(AddCharacter))
        {
            var gnode = CreateNode(AddCharacter.ID) as AddCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            gnode.targetPos = new Vector2(float.Parse(args[2]) * 8.8888f, float.Parse(args[3]) * 5f); // Normalized coordinates (hardcoded 16/9 res with camera size 5).
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
            gnode.bgType = (args[1].ToLower() == "sprite") ? SetBackgroundNode.BgType.Sprite 
                                                 : SetBackgroundNode.BgType.Prefab;
            string path = (args[1].ToLower() == "sprite") ? AssetDatabase.FindAssets(args[2], AssetDatabase.GetSubFolders("Assets/Graphics/Sprites/Backgrounds"))[0] 
                                                : AssetDatabase.FindAssets(args[2], AssetDatabase.GetSubFolders("Assets/Prefabs/Backgrounds"))[0];
            path = AssetDatabase.GUIDToAssetPath(path);
            Debug.Log("bg:" + path);
            if (args[1].ToLower() == "sprite") gnode.bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            else                     gnode.bgPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(FadeNode))
        {
            var gnode = CreateNode(FadeNode.ID) as FadeNode;
            gnode.fadeType = args[1] == "in" ? FadeNode.FadeType.Fade_In : FadeNode.FadeType.Fade_Out;
            gnode.fadeTime = float.Parse(args[2]);
            gnode.fadeColor = new Color(float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]), 1f);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(EndAndTransition))
        {
            var gnode = CreateNode(EndAndTransition.ID) as EndAndTransition;
            gnode.nextScene = args[1];
            nodes.Add(gnode);
        }
        return nodes;
    }

    // Parses a single dialog line. Returns constructed nodes.
    List<Node> ParseDialogNode(string line)
    {
        string[] dialogLine = line.Split(nameMarker, escape);
        List<Node> nodes = new List<Node>();
        string preMarker = dialogLine[0];
        // Remove specifiers from character name
        if (preMarker.Contains(displayNameChars, escape)) preMarker = preMarker.Substring(0, preMarker.IndexOfAny(displayNameChars, 0, escape)).Trim();
        if (preMarker.Contains(choiceMarker, escape)) preMarker = preMarker.Substring(0, preMarker.IndexOfAny(choiceMarker, 0, escape)).Trim();
        List<CharacterData> cds = new List<CharacterData>();
        List<string> exprs = new List<string>();
        List<string> poses = new List<string>();
        var clines = preMarker.Split(new char[] { '|' }, escape);
        string cname = "";
        foreach(var cline in clines)
        {
            string currname = (cline.Contains(exprMarker[0]) || cline.Contains(poseMarker[0]))
                ? cline.Substring(0, cline.IndexOfAny(exprMarker.Concat(poseMarker).ToArray(), 0, escape)).Trim()
                : cline.Trim();
            cname += currname + '|';
            var cd = GetCharacterData(currname);
            cds.Add(cd);
            #region Expression and pose
            string expr = string.Empty; // Expression string (value within parentheticals)
            string pose = string.Empty; // Pose string [value within square brackets]
            if (cline.Contains(poseMarker[0])) // Pose
            {
                pose = Regex.Match(cline, posePat).Value;
                pose = pose.Substring(1, pose.Length - 2);
            }
            if (cline.Contains(exprMarker[0])) // Expression
            {
                expr = Regex.Match(cline, exprPat).Value;
                expr = expr.Substring(1, expr.Length - 2);
            }
            exprs.Add(expr);
            poses.Add(pose);
            #endregion
        }
        string displayName = Regex.Match(dialogLine[0], displayPat).Value; // Displayed speaker name.
        displayName = (displayName.Length > 2) 
                    ? displayName.Substring(1, displayName.Length - 2) 
                    : "";
        DialogNode dnode = null; // Node for dialog.
        if (currView == typeof(DialogNodeVN) || currView == typeof(DialogNodeInput))
        {
            // Create expression and pose nodes
            for (int i = 0; i < cds.Count; i++)
            {
                if (cds[i] != null)
                {
                    if(!string.IsNullOrEmpty(poses[i]))
                    {
                        var hnode = CreateNode(SetPose.ID) as SetPose;
                        hnode.characterData = cds[i];
                        hnode.pose = poses[i];
                        nodes.Add(hnode);
                    }
                    if(!string.IsNullOrEmpty(exprs[i]))
                    {
                        var gnode = CreateNode(SetExpression.ID) as SetExpression;
                        gnode.characterData = cds[i];
                        gnode.expr = exprs[i];
                        nodes.Add(gnode);
                    }

                }
            }
            // Create dialog node
            if (currView == typeof(DialogNodeInput))
            {
                // Get choice prompts and variable name.
                string choices = Regex.Match(dialogLine[0], choicePat).Value;
                choices = choices.Substring(1, choices.Length - 2);
                var carr = choices.Split(new char[] { ',' }, escape);
                DialogNodeInput inode = CreateNode(DialogNodeInput.ID) as DialogNodeInput;
                inode.showChoicePrompt = carr.Length > 1;
                inode.variableName = carr[0];
                for (int i = 1; i < carr.Length; i++)
                    inode.choicePromptText[i - 1] = carr[i];
                dnode = inode;
            }
            else
            {
                dnode = CreateNode(DialogNodeVN.ID) as DialogNodeVN;
            }
        }
        else if (currView == typeof(DialogNodeChat))
        {
            // USE EXPRESSION TO CHANGE ICON (currently only 1 icon)
            dnode = CreateNode(DialogNodeChat.ID) as DialogNodeChat;
            if (poses[0] == "left")
            {
                (dnode as DialogNodeChat).leftIcon = cds[0].chat_icon;
            }
            else if (poses[0] == "right")
            {
                (dnode as DialogNodeChat).rightIcon = cds[0].chat_icon;
            }
            else if (poses[0] == "both")
            {
                (dnode as DialogNodeChat).leftIcon = cds[0].chat_icon;
                (dnode as DialogNodeChat).rightIcon = cds[0].chat_icon;
            }
        }
        else if (currView == typeof(DialogNodeAN))
        {
            dnode = CreateNode(DialogNodeAN.ID) as DialogNodeAN;
        }
        else if (currView == typeof(DialogNodeBubble))
        {
            dnode = CreateNode(DialogNodeBubble.ID) as DialogNodeBubble;
            (dnode as DialogNodeBubble).multi = (exprs[0] == "multi");
            var coords = poses[0].Split(',');
            (dnode as DialogNodeBubble).rectVal.x = float.Parse(coords[0]);
            (dnode as DialogNodeBubble).rectVal.y = float.Parse(coords[1]);
            (dnode as DialogNodeBubble).rectVal.width = float.Parse(coords[2]);
            (dnode as DialogNodeBubble).rectVal.height = float.Parse(coords[3]);
        }
        dnode.characterName = cname.Substring(0, cname.Length-1);
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
