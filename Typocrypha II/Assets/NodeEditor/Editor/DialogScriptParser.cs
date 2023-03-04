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
    public const string spellWordBundlePath = "Assets/ScriptableObjects/Bundles/AllWordsBundle.asset";
    TextAsset textScript; // Text script asset
    NodeCanvas canvas; // Generated canvas
    bool endAndTransition = true;
    System.Type currView = typeof(DialogViewVNPlus); // Current dialog view
    float pos; // Position of current node
    Node prev;

    AssetBundle characterDataBundle; // Character data bundle
    SpellWordBundle spellWords; // All spell words
    CharacterData[] allCharacterData; // All loaded character data

    readonly char[] lineDelim = new char[] { '\n' }; // Line delimiter.
    readonly char[] argDelim = new char[] { ',' }; // List of arguments delimiter.
    const char nameMarker = ':'; // Speaker's name marker for dialog lines.
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
    readonly string googleCommentPat = @"(\w)(?:\[\w\])+";

    private const string spriteBgPath = "Assets/Graphics/Sprites/Backgrounds";
    private const string prefabBgPath = "Assets/Prefabs/Backgrounds";

    // Dialog view labels.
    Dictionary<string, System.Type> viewMap => SetDialogViewNode.viewMap;

    // General node labels.
    Dictionary<string, System.Type> nodeMap = new Dictionary<string, System.Type>
    {
        {"addchar", typeof(AddCharacter) },
        {"removechar", typeof(RemoveCharacter) },
        {"playbgm", typeof(PlayBgm) },
        {"stopbgm", typeof(StopBgm) },
        {"setbg", typeof(SetBackgroundNode) },
        {"fade", typeof(FadeNode) },
        {"start" , typeof(GameflowStartNode) },
        {"end", typeof(EndAndHide) },
        {"endt", typeof(EndAndTransition) },
        {"setvar", typeof(SetVariableNode) },
        {"setexpr", typeof(SetExpression) },
        {"setexpression", typeof(SetExpression) },
        {"setpose", typeof(SetPose) },
        {"setlocation", typeof(SetLocationTextNode) },
        {"setlocationtext", typeof(SetLocationTextNode) },
        {"setdatetimetext", typeof(SetDateTimeTextNode) },
        {"setdatetime", typeof(SetDateTimeTextNode) },
        {"setdate", typeof(SetDateTimeTextNode) },
        {"settime", typeof(SetDateTimeTextNode) },
        {"cast", typeof(CastSpellNode) },
        {"castSpell", typeof(CastSpellNode) },
        {"clear", typeof(ClearNode) },
        {"clearSpells", typeof(ClearEquippedSpellsNode) },
        {"addSpell", typeof(AddEquippedSpellsNode) },
        {"addSpells", typeof(AddEquippedSpellsNode) },
    };

    // Generic node ID map. types that have entries in this map and the nodeMap can be created without additional partsing code
    // This should only be done with nodes that have no arguments
    readonly Dictionary<System.Type, string> nodeIDMap = new Dictionary<System.Type, string>
    {
        {typeof(ClearNode), ClearNode.ID },
        {typeof(ClearEquippedSpellsNode), ClearEquippedSpellsNode.ID },
        {typeof(EndAndTransition), EndAndTransition.ID },
        {typeof(EndAndHide), EndAndHide.ID },
    };

    AnimationCurve bgmFadeIn = AnimationCurve.Constant(0,0,1);//AnimationCurve.EaseInOut(0, 0, 0.1, 1); // Default fade in curve
    AnimationCurve bgmFadeOut = AnimationCurve.EaseInOut(0, 1, 1, 0); // Default fade out curve

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

        endAndTransition = EditorGUILayout.ToggleLeft("Use End and Transition as Default End Node", endAndTransition);

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
        spellWords = AssetDatabase.LoadAssetAtPath<SpellWordBundle>(spellWordBundlePath);
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
        currView = viewMap["vnplus"]; // Default to visual novel view
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
        text = Regex.Replace(text, googleCommentPat, "$1");
        text = Regex.Replace(text, "/{2}.*?\n", "\n"); // Remove comments
        text = Regex.Replace(text, "/{2}.*", ""); // Remove comments (end of file)
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
                if(ParseLine(lines[i], ref prev))
                {
                    break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Line Error{(i + 1)}: {lines[i]}: {e}");
            }
        }
        // Create end node (if not already there)
        CreateEndNodeIfNeeded(prev);
        EndCanvas();
    }

    private void CreateEndNodeIfNeeded(Node prev)
    {
        if (prev is GameflowEndNode)
            return;
        var endNode = (endAndTransition ? CreateNode(EndAndTransition.ID) : CreateNode(EndAndHide.ID)) as GameflowEndNode;
        (prev as BaseNodeIO).toNextOUT.TryApplyConnection(endNode.fromPreviousIN, true);
    }
    // Parses a single line
    bool ParseLine(string line, ref Node prev)
    {
        line = line.Trim();
        if (line.Length < 2) return false; // Empty line.
        if (line.StartsWith("["))
            return true;
        List<Node> nodes = null; // Constructed nodes.
        if (viewSwitchMarker.Contains(line[0])) // View switch.
        {
            string[] dialogLine = line.Split(viewSwitchMarker, escape);
            currView = viewMap[dialogLine[1]];
            nodes = ParseViewSwitchNode(dialogLine[1]);
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
        return false;
    }

    List<Node> ParseViewSwitchNode(string viewName)
    {
        var nodes = new List<Node>();
        var switchNode = CreateNode(SetDialogViewNode.Id) as SetDialogViewNode;
        switchNode.viewName = viewName;
        nodes.Add(switchNode);
        return nodes;
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
            if (canvas != null)
            {
                // Create end node (if not already there)
                CreateEndNodeIfNeeded(prev);
                EndCanvas(); // End previous canvas.
            }
            StartCanvas(args[1]); // Start new canvas.
        }
        else if (nodeType == typeof(AddCharacter))
        {
            var gnode = CreateNode(AddCharacter.ID) as AddCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            gnode.targetPos = new Vector2(float.Parse(args[2]) * 8.8888f, float.Parse(args[3]) * 5f); // Normalized coordinates (hardcoded 16/9 res with camera size 5).
            if(args.Length > 4)
            {
                gnode.column = args[4].Trim().ToLower() == "left" ? DialogViewVNPlus.CharacterColumn.Left : DialogViewVNPlus.CharacterColumn.Right;
            }
            if(args.Length == 6)
            {
                gnode.initialExpr = args[5];
            }
            else if(args.Length >= 7)
            {
                gnode.initialPose = args[5];
                gnode.initialExpr = args[6];
            }
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(RemoveCharacter))
        {
            var gnode = CreateNode(RemoveCharacter.ID) as RemoveCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(SetExpression))
        {
            var node = CreateNode(SetExpression.ID) as SetExpression;
            node.characterData = GetCharacterData(args[1]);
            node.expr = args[2];
            nodes.Add(node);
        }
        else if (nodeType == typeof(SetPose))
        {
            var node = CreateNode(SetPose.ID) as SetPose;
            node.characterData = GetCharacterData(args[1]);
            node.pose = args[2];
            nodes.Add(node);
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
            bool isSprite = args[1].ToLower() == "sprite";
            gnode.bgType = isSprite ? SetBackgroundNode.BgType.Sprite : SetBackgroundNode.BgType.Prefab;
            string[] folders = GetPathWithSubFolders(isSprite ? spriteBgPath : prefabBgPath);
            // Find asset path
            string path = AssetDatabase.FindAssets(args[2], folders)[0];
            path = AssetDatabase.GUIDToAssetPath(path);
            Debug.Log("bg:" + path);
            // Load actual asset
            if (isSprite) gnode.bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            else          gnode.bgPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
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
        else if(nodeType == typeof(SetVariableNode))
        {
            var gnode = CreateNode(SetVariableNode.Id) as SetVariableNode;
            gnode.variableName = args[1];
            gnode.value = args[2];
            nodes.Add(gnode);
        }
        else if(nodeType == typeof(SetLocationTextNode))
        {
            var gnode = CreateNode(SetLocationTextNode.Id) as SetLocationTextNode;
            gnode.text = args[1];
            nodes.Add(gnode);
        }
        else if(nodeType == typeof(SetDateTimeTextNode))
        {
            var gnode = CreateNode(SetDateTimeTextNode.Id) as SetDateTimeTextNode;
            gnode.text = args[1];
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(CastSpellNode))
        {
            var castNode = CreateNode(CastSpellNode.Id) as CastSpellNode;
            if(args.Length < 6)
            {
                throw new System.Exception($"Incorrect number of args for cast spell node ({args.Length - 1}). Expected at least 5");
            }
            // Parse spell
            var spellWordStrings = args[1].Split(Player.separator);
            TryParseSpellWord(spellWordStrings, 0, out castNode.word1, CastSpellNode.Id);
            TryParseSpellWord(spellWordStrings, 1, out castNode.word2, CastSpellNode.Id);
            TryParseSpellWord(spellWordStrings, 2, out castNode.word3, CastSpellNode.Id);

            // Parse other data
            castNode.targetPos = new Vector2Int(int.Parse(args[3]), int.Parse(args[2]));
            castNode.casterPos = new Vector2Int(int.Parse(args[5]), int.Parse(args[4]));
            if(args.Length >= 7)
            {
                castNode.messageOverride = args[6];
            }
            nodes.Add(castNode);
        }
        else if(nodeType == typeof(AddEquippedSpellsNode))
        {
            var addSpellsNode = CreateNode(AddEquippedSpellsNode.Id) as AddEquippedSpellsNode;
            if(args.Length < 2)
            {
                throw new System.Exception($"Incorrect number of args for add spell node ({args.Length - 1}). Expected at least 2");
            }
            TryParseSpellWord(args[1], out addSpellsNode.word1, AddEquippedSpellsNode.Id);
            if(args.Length >= 3)
            {
                TryParseSpellWord(args[2], out addSpellsNode.word2, AddEquippedSpellsNode.Id);
            }
            if (args.Length >= 4)
            {
                TryParseSpellWord(args[3], out addSpellsNode.word3, AddEquippedSpellsNode.Id);
            }
            nodes.Add(addSpellsNode);
        }
        else
        {
            var gnode = CreateNodeGeneric(nodeType);
            if(gnode != null)
            {
                nodes.Add(gnode);
            }
        }
        return nodes;
    }



    // Parses a single dialog line. Returns constructed nodes.
    List<Node> ParseDialogNode(string line)
    {
        int nameMarkerIndex = line.IndexOf(nameMarker);
        int dialogStartIndex = nameMarkerIndex + 1;
        string[] dialogLine = new string[] { nameMarkerIndex == -1 ? "" : line.Substring(0, nameMarkerIndex), line.Substring(dialogStartIndex, line.Length - dialogStartIndex)};
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
        if (currView == typeof(DialogViewVN) || currView == typeof(DialogNodeInput) || currView == typeof(DialogViewVNPlus))
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
            else if(currView == typeof(DialogNodeVN))
            {
                dnode = CreateNode(DialogNodeVN.ID) as DialogNodeVN;
            }
            else
            {
                dnode = CreateNode(DialogNodeVNPlus.ID) as DialogNodeVNPlus;
            }
        }
        else if (currView == typeof(DialogViewChat))
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
        else if (currView == typeof(DialogViewAN))
        {
            var dnodeAN = CreateNode(DialogNodeAN.ID) as DialogNodeAN;

            if(exprs.Count > 0)
            {
                string alignment = exprs[0];
                if(alignment == "left")
                {
                    dnodeAN.alignmentOptions = TMPro.TextAlignmentOptions.Left;
                }
                else if(alignment == "right")
                {
                    dnodeAN.alignmentOptions = TMPro.TextAlignmentOptions.Right;
                }
                else if (alignment == "center")
                {
                    dnodeAN.alignmentOptions = TMPro.TextAlignmentOptions.Center;
                }
            }
            if(poses.Count > 0)
            {
                string layout = poses[0];
                if (layout == "upper" || layout == "top")
                {
                    dnodeAN.layoutSetting = TextAnchor.UpperLeft;
                }
                else if (layout == "lower" || layout == "bottom")
                {
                    dnodeAN.layoutSetting = TextAnchor.LowerLeft;
                }
                else if (layout == "middle" || layout == "center")
                {
                    dnodeAN.layoutSetting = TextAnchor.MiddleLeft;
                }
            }
            dnode = dnodeAN;
        }
        else if (currView == typeof(DialogViewBubble))
        {
            var bubbleNode = CreateNode(DialogNodeBubble.ID) as DialogNodeBubble;
            dnode = bubbleNode;
            //(dnode as DialogNodeBubble).multi = (exprs[0] == "multi");
            // Pose / Expression Change
            if (cds.Count > 0 && cds[0] != null)
            {
                if (exprs.Count > 0 && !string.IsNullOrWhiteSpace(exprs[0]))
                {
                    var exprData = exprs[0].Split(',');
                    if (exprData.Length == 1)
                    {
                        // Just an expression setter
                        var setExprNode = CreateNode(SetExpression.ID) as SetExpression;
                        setExprNode.characterData = cds[0];
                        setExprNode.expr = exprs[0];
                        nodes.Add(setExprNode);
                    }
                    else if (exprData.Length == 2)
                    {
                        // Pose + expression
                        var setPoseNode = CreateNode(SetPose.ID) as SetPose;
                        setPoseNode.characterData = cds[0];
                        setPoseNode.pose = exprData[0];
                        nodes.Add(setPoseNode);

                        // (Optional) expression change
                        if (!string.IsNullOrWhiteSpace(exprData[1]))
                        {
                            var setExprNote = CreateNode(SetExpression.ID) as SetExpression;
                            setExprNote.characterData = cds[1];
                            setExprNote.expr = exprs[1];
                            nodes.Add(setExprNote);
                        }
                    }
                    else
                    {
                        throw new System.Exception($"Incorrect number of expr arguments ({exprData.Length}) for bubble dialog line");
                    }
                }
            }
            // Actual Dialog Node
            var coords = poses[0].Split(',');
            if (coords.Length == 1)
            {
                throw new System.NotImplementedException("Special bubble location aliases not yet supported. Use grid coords or absolute positioning");
            }
            else if (coords.Length == 2) // Battle grid coords (0 is bottom row, 0 is left col)
            {
                bubbleNode.gridPosition = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));
            }
            else if (coords.Length == 4)// absolute position specification
            {
                bubbleNode.gridPosition = new Vector2Int(int.Parse(coords[0]), int.Parse(coords[1]));
                bubbleNode.absolutePosition = new Vector2(float.Parse(coords[2]), float.Parse(coords[3]));
            }
            else
            {
                throw new System.Exception($"Incorrect number of arguments ({coords.Length}) for bubble dialog");
            }
        }
        dnode.characterName = cname.Substring(0, cname.Length-1);
        dnode.displayName = displayName;
        dnode.text = dialogLine[1].Trim().Replace("…", "...").Replace('’', '\'');
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

    private Node CreateNodeGeneric(System.Type nodeType)
    {
        if(!nodeIDMap.TryGetValue(nodeType, out var id))
        {
            return null;
        }
        return CreateNode(id);
    }

    private static string[] GetPathWithSubFolders(string path)
    {
        var sub = AssetDatabase.GetSubFolders(path);
        var folders = new string[sub.Length + 1];
        folders[0] = path;
        System.Array.Copy(sub, 0, folders, 1, sub.Length);
        return folders;
    }

    private void TryParseSpellWord(string[] words, int index, out SpellWord output, string nodeName)
    {
        if (words.Length > index)
        {
            TryParseSpellWord(words[index], out output, nodeName);
        }
        else
        {
            output = null;
        }
    }

    private void TryParseSpellWord(string word, out SpellWord output, string nodeName)
    {
        if (!spellWords.words.TryGetValue(word.Trim().ToLower(), out output))
        {
            throw new System.Exception($"Spell word \"{word}\" doesn't exist! Node type: {nodeName}");
        }
    }
}
