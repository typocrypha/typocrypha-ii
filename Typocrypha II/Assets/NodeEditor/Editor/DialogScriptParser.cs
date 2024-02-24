﻿using Gameflow;
using NodeEditorFramework;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Typocrypha;
using UnityEditor;
using UnityEngine;

// Syntax: https://docs.google.com/document/d/1za4Xt3NwA6cOirZnnqCrD0kbA193A7Ypk6n_yVwa1e4/edit#heading=h.jg4trxckxznj

/// <summary>
/// Parses text scripts into dialog node canvases
/// </summary>
public class DialogScriptParser
{
    public const string assetPath = "Assets/ScriptableObjects/DialogScenes/";
    public const string spellWordBundlePath = "Assets/ScriptableObjects/Bundles/AllWordsBundle.asset";
    public const string allyBundlePath = "Assets/ScriptableObjects/Bundles/AllyBundle.asset";
    public const string characterDataPath = "Assets/ScriptableObjects/CharacterData";
    System.Type currView = typeof(DialogViewVNPlus); // Current dialog view
    float pos; // Position of current node
    Node prev;

    SpellWordBundle spellWords; // All spell words
    PrefabBundle allyBundle; // Ally indexable allies
    IReadOnlyList<CharacterData> allCharacterData; // All loaded character data

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
    readonly string googleCommentPat = @"(.)(?:\[\w\])+";

    private const string spriteBgPath = "Assets/Graphics/Sprites/Backgrounds";
    private const string prefabBgPath = "Assets/Prefabs/Backgrounds";

    // Dialog view labels.
    Dictionary<string, System.Type> viewMap => SetDialogViewNode.viewMap;

    // General node labels.
    Dictionary<string, System.Type> nodeMap = new Dictionary<string, System.Type>
    {
        {"addchar", typeof(AddCharacter) },
        {"addcharmulti", typeof(AddCharacterMulti) },
        {"addcharmultiargs", typeof(AddCharacterMulti) },
        {"removechar", typeof(RemoveCharacter) },
        {"removecharmulti", typeof(RemoveCharacterMulti) },
        {"movechar", typeof(MoveCharacter) },
        {"embedimage", typeof(EmbedImage) },
        {"playbgm", typeof(PlayBgm) },
        {"stopbgm", typeof(StopBgm) },
        {"pausebgm", typeof(PauseBgm) },
        {"unpausebgm", typeof(PauseBgm) },
        {"crossfadebgm", typeof(CrossfadeBgm) },
        {"playsfx", typeof(PlaySfx) },
        {"setbg", typeof(SetBackgroundNode) },
        {"movecam", typeof(MoveCameraNode) },
        {"setcam", typeof(SetCameraNode) },
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
        {"castSpellProxy", typeof(CastSpellNode) },
        {"castSpellName", typeof(CastSpellNode) },
        {"castSpellAlly", typeof(CastSpellNode) },
        {"setAlly", typeof(SetAllyNode) },
        {"clear", typeof(ClearNode) },
        {"clearReinforcements", typeof(ClearReinforcementsNode) },
        {"clearSpells", typeof(ClearEquippedSpellsNode) },
        {"addSpell", typeof(AddEquippedSpellsNode) },
        {"addSpells", typeof(AddEquippedSpellsNode) },
        {"wait", typeof(PauseNode) }
    };

    // Generic node ID map. types that have entries in this map and the nodeMap can be created without additional parsing code
    // This should only be done with nodes that have no arguments
    readonly Dictionary<System.Type, string> nodeIDMap = new Dictionary<System.Type, string>
    {
        {typeof(ClearNode), ClearNode.ID },
        {typeof(ClearReinforcementsNode), ClearReinforcementsNode.ID },
        {typeof(ClearEquippedSpellsNode), ClearEquippedSpellsNode.ID },
        {typeof(EndAndTransition), EndAndTransition.ID },
        {typeof(EndAndHide), EndAndHide.ID },
    };

    private const float defaultBgmFadeLength = 1.0f;
    AnimationCurve bgmFadeInDefault = AnimationCurve.Constant(0,0,1); // Default fade in curve
    AnimationCurve bgmFadeOutDefault = AnimationCurve.EaseInOut(0, 1, defaultBgmFadeLength, 0); // Default fade out curve
    readonly Dictionary<string, AnimationCurve> customFadeCurves = new Dictionary<string, AnimationCurve>();

    const float nodeSpacing = 40f;

    // Generates node canvases from script
    public NodeCanvas GenerateCanvas(TextAsset asset, bool endAndTransition)
    {
        spellWords = AssetDatabase.LoadAssetAtPath<SpellWordBundle>(spellWordBundlePath);
        allyBundle = AssetDatabase.LoadAssetAtPath<PrefabBundle>(allyBundlePath);
        allCharacterData = AssetUtils.LoadAllAssetsInDirectoryRecursive<CharacterData>(characterDataPath);
        return Parse(asset.text, endAndTransition);
    }

    /// <summary>
    /// Attempts to make a new canvas with the given name, or finds the existing canvas.
    /// Sets the canvas to the 'canvas' field.
    /// </summary>
    /// <param name="canvasName"></param>
    NodeCanvas StartCanvas(string canvasName)
    {
        NodeCanvas canvas = AssetDatabase.LoadAssetAtPath<DialogCanvas>(assetPath + canvasName + ".asset");
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
        prev = CreateNode(canvas, GameflowStartNode.ID) as GameflowStartNode;
        currView = viewMap["vnplus"]; // Default to visual novel view
        return canvas;
    }

    /// <summary>
    /// Saves current canvas.
    /// </summary>
    void EndCanvas(NodeCanvas canvas)
    {
        canvas.saveName = assetPath + canvas.name + ".asset";
        NodeEditorSaveManager.SaveNodeCanvas(canvas.saveName, ref canvas, true);
    }

    // Parses text into node canvas
    NodeCanvas Parse(string text, bool endAndTransition)
    {
        #region Preprocessing text
        text = Regex.Replace(text, googleCommentPat, "$1");
        text = Regex.Replace(text, "/{2}.*?\n", "\n"); // Remove comments
        text = Regex.Replace(text, "/{2}.*", ""); // Remove comments (end of file)
        text = Regex.Replace(text, "/[\x2A].*?[\x2A]/", "", RegexOptions.Singleline); // Remove block comments
        text = Regex.Replace(text, "\r", ""); // Remove carriage returns
        text = Regex.Replace(text, ":\\s*\n", ":"); // Remove newlines after character name delimiters.
        #endregion
        string[] lines = text.Split(lineDelim, escape); // Separate lines
        NodeCanvas canvas = null;

        for (int i = 0; i < lines.Length; i++)
        {
            //Debug.Log("parsing:" + (i+1) +":" + lines[i]);
            try
            {
                if(ParseLine(ref canvas, lines[i], endAndTransition, ref prev))
                {
                    break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Line Error {(i + 1)}: {lines[i]}: {e} {e.StackTrace}");
            }
        }
        // Create end node (if not already there)
        CreateEndNodeIfNeeded(canvas, prev, endAndTransition);
        EndCanvas(canvas);
        return canvas;
    }

    private void CreateEndNodeIfNeeded(NodeCanvas canvas, Node prev, bool endAndTransition)
    {
        if (prev is GameflowEndNode)
            return;
        var endNode = (endAndTransition ? CreateNode(canvas, EndAndTransition.ID) : CreateNode(canvas, EndAndHide.ID)) as GameflowEndNode;
        (prev as BaseNodeIO).toNextOUT.TryApplyConnection(endNode.fromPreviousIN, true);
    }

    // Parses a single line
    bool ParseLine(ref NodeCanvas canvas, string line, bool endAndTransition, ref Node prev)
    {
        line = line.Trim();
        if (line.Length < 2) return false; // Empty line.
        if (line.StartsWith("["))
            return true;
        List<Node> nodes; // Constructed nodes.
        if (viewSwitchMarker.Contains(line[0])) // View switch.
        {
            string[] dialogLine = line.Split(viewSwitchMarker, escape);
            currView = viewMap[dialogLine[1]];
            nodes = ParseViewSwitchNode(canvas, dialogLine[1]);
        }
        else if (nodeMarker.Contains(line[0])) // General node.
        {
            nodes = ParseGeneralNode(ref canvas, line, endAndTransition); // Parse node.
        }
        else // Line of dialog.
        {
            nodes = ParseDialogNode(canvas, line); // Parse dialog
        }
        if (nodes != null)
        {
            foreach(var node in nodes)
            {
                var prevOut = prev as BaseNodeOUT;
                // Connect to previous
                if (node is BaseNodeIO ioNode)
                {
                    prevOut.toNextOUT.TryApplyConnection(ioNode.fromPreviousIN, true);
                }
                else if(node is GameflowEndNode gameflowEndNode)
                {
                    prevOut.toNextOUT.TryApplyConnection(gameflowEndNode.fromPreviousIN, true);
                }
                prev = node;
            }
        }
        return false;
    }

    List<Node> ParseViewSwitchNode(NodeCanvas canvas, string viewName)
    {
        var nodes = new List<Node>();
        var switchNode = CreateNode(canvas, SetDialogViewNode.Id) as SetDialogViewNode;
        switchNode.viewName = viewName;
        nodes.Add(switchNode);
        return nodes;
    }

    // Parses a general node (not dialog) line. Returns constructed nodes.
    List<Node> ParseGeneralNode(ref NodeCanvas canvas, string line, bool endAndTransition)
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
                CreateEndNodeIfNeeded(canvas, prev, endAndTransition);
                EndCanvas(canvas); // End previous canvas.
            }
            canvas = StartCanvas(args[1]); // Start new canvas.
        }
        else if (nodeType == typeof(AddCharacter))
        {
            var gnode = CreateNode(canvas, AddCharacter.ID) as AddCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            gnode.targetPos = new Vector2(float.Parse(args[2]) * 8.8888f, float.Parse(args[3]) * 5f); // Normalized coordinates (hardcoded 16/9 res with camera size 5).
            if (args.Length > 4)
            {
                gnode.column = args[4].Trim().ToLower() == "left" ? DialogView.CharacterColumn.Left : DialogView.CharacterColumn.Right;
            }
            if (args.Length == 6)
            {
                gnode.initialExpr = args[5];
            }
            else if (args.Length >= 7)
            {
                gnode.initialPose = args[5];
                gnode.initialExpr = args[6];
            }
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(AddCharacterMulti))
        {
            var node = CreateNode(canvas, AddCharacterMulti.ID) as AddCharacterMulti;
            node.column = args[1].Trim().ToLower() == "left" ? DialogView.CharacterColumn.Left : DialogView.CharacterColumn.Right;
            node.characterData1 = GetCharacterData(args[2]);
            if (args[0] == "addcharmulti")
            {
                if(args.Length >= 4)
                {
                    node.characterData2 = GetCharacterData(args[3]);
                    if (args.Length >= 5)
                    {
                        node.characterData3 = GetCharacterData(args[4]);
                    }
                }
            }
            else if(args.Length >= 4) // Add character has arguments, use complex parsing
            {
                int i = 3;
                int subArgumentLength = int.Parse(args[i++]);
                if(subArgumentLength == 1)
                {
                    node.initialExpr1 = args[i++];
                }
                else if(subArgumentLength == 2)
                {
                    node.initialPose1 = args[i++];
                    node.initialExpr1 = args[i++];
                }
                if(i + 1 < args.Length)
                {
                    node.characterData2 = GetCharacterData(args[i++]);
                    subArgumentLength = int.Parse(args[i++]);
                    if (subArgumentLength == 1)
                    {
                        node.initialExpr2 = args[i++];
                    }
                    else if (subArgumentLength == 2)
                    {
                        node.initialPose2 = args[i++];
                        node.initialExpr2 = args[i++];
                    }
                    if(i + 1 < args.Length)
                    {
                        node.characterData3 = GetCharacterData(args[i++]);
                        subArgumentLength = int.Parse(args[i++]);
                        if (subArgumentLength == 1)
                        {
                            node.initialExpr3 = args[i++];
                        }
                        else if (subArgumentLength == 2)
                        {
                            node.initialPose3 = args[i++];
                            node.initialExpr3 = args[i++];
                        }
                    }
                }
            }
            nodes.Add(node);
        }
        else if (nodeType == typeof(RemoveCharacter))
        {
            var gnode = CreateNode(canvas, RemoveCharacter.ID) as RemoveCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(RemoveCharacterMulti))
        {
            var node = CreateNode(canvas, RemoveCharacterMulti.ID) as RemoveCharacterMulti;
            node.characterData1 = GetCharacterData(args[1]);
            if(args.Length >= 3)
            {
                node.characterData2 = GetCharacterData(args[2]);
                if (args.Length >= 4)
                {
                    node.characterData3 = GetCharacterData(args[3]);
                }
            }
            nodes.Add(node);
        }
        else if (nodeType == typeof(MoveCharacter))
        {
            var gnode = CreateNode(canvas, MoveCharacter.ID) as MoveCharacter;
            gnode.characterData = GetCharacterData(args[1]);
            gnode.targetColumn = args[2].Trim().ToLower() == "right" ? DialogView.CharacterColumn.Right : DialogView.CharacterColumn.Left;
            gnode.top = args[3].Trim().ToLower() == "top";
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(EmbedImage))
        {
            var gnode = CreateNode(canvas, EmbedImage.ID) as EmbedImage;
            gnode.sprite = LoadAsset<Sprite>(args[1], "Assets/Graphics/Sprites/Dialog");
            if (args.Length == 3) gnode.messagesBeforeFade = int.Parse(args[2]);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(SetExpression))
        {
            var node = CreateNode(canvas, SetExpression.ID) as SetExpression;
            node.characterData = GetCharacterData(args[1]);
            node.expr = args[2];
            nodes.Add(node);
        }
        else if (nodeType == typeof(SetPose))
        {
            var node = CreateNode(canvas, SetPose.ID) as SetPose;
            node.characterData = GetCharacterData(args[1]);
            node.pose = args[2];
            nodes.Add(node);
        }
        else if (nodeType == typeof(PlayBgm))
        {
            var gnode = CreateNode(canvas, PlayBgm.ID) as PlayBgm;
            gnode.bgm = LoadAsset<AudioClip>(args[1], "Assets/Audio/Clips/BGM");
            // If there aren't enough args to contain a fade curve or the curve parse fails, use the default
            if (args.Length == 3)
            {
                if (!TryParseFadeCurve(args[2], defaultBgmFadeLength, true, out gnode.fadeCurve))
                {
                    gnode.fadeCurve = bgmFadeInDefault;
                }
            }
            else if (args.Length >= 4)
            {
                if (!TryParseFadeCurve(args[2], args[3], true, out gnode.fadeCurve))
                {
                    gnode.fadeCurve = bgmFadeInDefault;
                }
            }
            else
            {
                gnode.fadeCurve = bgmFadeInDefault;
            }
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(StopBgm))
        {
            var gnode = CreateNode(canvas, StopBgm.ID) as StopBgm;
            // If there aren't enough args to contain a fade curve or the curve parse fails, use the default
            if (args.Length == 2)
            {
                if (!TryParseFadeCurve(args[1], defaultBgmFadeLength, false, out gnode.fadeCurve))
                {
                    gnode.fadeCurve = bgmFadeOutDefault;
                }
            }
            else if (args.Length >= 3)
            {
                if (!TryParseFadeCurve(args[1], args[2], false, out gnode.fadeCurve))
                {
                    gnode.fadeCurve = bgmFadeOutDefault;
                }
            }
            else
            {
                gnode.fadeCurve = bgmFadeOutDefault;
            }
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(PauseBgm))
        {
            var pauseBgmNode = CreateNode(canvas, PauseBgm.ID) as PauseBgm;
            pauseBgmNode.pause = args[0] != "unpausebgm";
            nodes.Add(pauseBgmNode);
        }
        else if (nodeType == typeof(CrossfadeBgm))
        {
            var node = CreateNode(canvas, CrossfadeBgm.ID) as CrossfadeBgm;
            node.bgm = LoadAsset<AudioClip>(args[1], "Assets/Audio/Clips/BGM");
            switch (args.Length)
            {
                case 3:
                    if (float.TryParse(args[2], out float time))
                    {
                        node.fadeCurveIn = AnimationCurve.Linear(0, 0, time, 1);
                        node.fadeCurveOut = AnimationCurve.Linear(0, 1, time, 0);
                    }
                    else
                    {
                        TryParseFadeCurve(args[2], defaultBgmFadeLength, true, out node.fadeCurveIn);
                        TryParseFadeCurve(args[2], defaultBgmFadeLength, false, out node.fadeCurveOut);
                    }
                    break;
                case 4:
                    var arg2IsTime = float.TryParse(args[2], out float time1);
                    var arg3IsTime = float.TryParse(args[3], out float time2);
                    if (arg2IsTime && arg3IsTime)
                    {
                        node.fadeCurveIn = AnimationCurve.Linear(0, 0, time1, 1);
                        node.fadeCurveOut = AnimationCurve.Linear(0, 1, time2, 0);
                    }
                    else if (!arg2IsTime && arg3IsTime)
                    {
                        TryParseFadeCurve(args[2], time2, true, out node.fadeCurveIn);
                        TryParseFadeCurve(args[2], time2, false, out node.fadeCurveOut);
                    }
                    else if (!arg2IsTime && !arg3IsTime)
                    {
                        TryParseFadeCurve(args[2], defaultBgmFadeLength, true, out node.fadeCurveIn);
                        TryParseFadeCurve(args[3], defaultBgmFadeLength, false, out node.fadeCurveOut);
                    }
                    break;
                case 6:
                    TryParseFadeCurve(args[2], float.TryParse(args[4], out time1) ? time1 : defaultBgmFadeLength, true, out node.fadeCurveIn);
                    TryParseFadeCurve(args[3], float.TryParse(args[5], out time2) ? time2 : defaultBgmFadeLength, false, out node.fadeCurveOut);
                    break;
            }
            nodes.Add(node);
        }
        else if (nodeType == typeof(PlaySfx))
        {
            var playSfxNode = CreateNode(canvas, PlaySfx.ID) as PlaySfx;
            playSfxNode.sfx = LoadAsset<AudioClip>(args[1], "Assets/Audio/Clips/SFX");
            nodes.Add(playSfxNode);
        }
        else if (nodeType == typeof(SetBackgroundNode))
        {
            var gnode = CreateNode(canvas, SetBackgroundNode.ID) as SetBackgroundNode;
            bool isSprite = args[1].ToLower() == "sprite";
            gnode.bgType = isSprite ? SetBackgroundNode.BgType.Sprite : SetBackgroundNode.BgType.Prefab;
            string[] folders = GetPathWithSubFolders(isSprite ? spriteBgPath : prefabBgPath);
            // Find asset path
            string path = AssetDatabase.FindAssets(args[2], folders)[0];
            path = AssetDatabase.GUIDToAssetPath(path);
            //Debug.Log("bg:" + path);
            // Load actual asset
            if (isSprite) gnode.bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            else gnode.bgPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(MoveCameraNode))
        {
            var camNode = CreateNode(canvas, MoveCameraNode.ID) as MoveCameraNode;
            camNode.StartPivot = new Vector2(float.Parse(args[1]), float.Parse(args[2]));
            camNode.FinalPivot = new Vector2(float.Parse(args[3]), float.Parse(args[4]));
            camNode.Duration = float.Parse(args[5]);
            TryParseFadeCurve(args[6], 1f, true, out camNode.EasingCurve);
            nodes.Add(camNode);
        }
        else if (nodeType == typeof(SetCameraNode))
        {
            var camNode = CreateNode(canvas, SetCameraNode.ID) as SetCameraNode;
            camNode.Pivot = new Vector2(float.Parse(args[1]), float.Parse(args[2]));
            nodes.Add(camNode);
        }
        else if (nodeType == typeof(FadeNode))
        {
            var gnode = CreateNode(canvas, FadeNode.ID) as FadeNode;
            gnode.fadeType = args[1] == "in" ? FadeNode.FadeType.FadeIn : FadeNode.FadeType.FadeOut;
            gnode.fadeTime = float.Parse(args[2]);
            gnode.fadeColor = new Color(float.Parse(args[3]), float.Parse(args[4]), float.Parse(args[5]), 1f);
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(SetVariableNode))
        {
            var gnode = CreateNode(canvas, SetVariableNode.Id) as SetVariableNode;
            gnode.variableName = args[1];
            gnode.value = args[2];
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(SetLocationTextNode))
        {
            var gnode = CreateNode(canvas, SetLocationTextNode.Id) as SetLocationTextNode;
            gnode.text = args[1];
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(SetDateTimeTextNode))
        {
            var gnode = CreateNode(canvas, SetDateTimeTextNode.Id) as SetDateTimeTextNode;
            gnode.text = args[1];
            nodes.Add(gnode);
        }
        else if (nodeType == typeof(CastSpellNode))
        {
            var castNode = CreateNode(canvas, CastSpellNode.Id) as CastSpellNode;
            if (args.Length < 4)
            {
                throw new System.Exception($"Incorrect number of args for cast spell node ({args.Length - 1}). Expected at least 3");
            }
            // Parse spell
            var spellWordStrings = args[1].Split(CastBar.KeywordDelimiters);
            TryParseSpellWord(spellWordStrings, 0, out castNode.word1, CastSpellNode.Id);
            TryParseSpellWord(spellWordStrings, 1, out castNode.word2, CastSpellNode.Id);
            TryParseSpellWord(spellWordStrings, 2, out castNode.word3, CastSpellNode.Id);

            // Parse other data
            castNode.targetPos = new Vector2Int(int.Parse(args[3]), int.Parse(args[2]));
            if (args[0] == "castSpellProxy")
            {
                castNode.proxyCasterName = args[4];
                castNode.searchField = false;
                if (args.Length >= 6)
                {
                    castNode.messageOverride = args[5];
                }
            }
            else if (args[0] == "castSpellName")
            {
                castNode.proxyCasterName = args[4];
                castNode.searchField = true;
                if (args.Length >= 6)
                {
                    castNode.messageOverride = args[5];
                }
            }
            else if (args[0] == "castSpellAlly")
            {
                castNode.casterPos = new Vector2Int(2, 1);
                if (args.Length >= 5)
                {
                    castNode.messageOverride = args[4];
                }
            }
            else
            {
                castNode.casterPos = new Vector2Int(int.Parse(args[5]), int.Parse(args[4]));
                if (args.Length >= 7)
                {
                    castNode.messageOverride = args[6];
                }
            }
            nodes.Add(castNode);
        }
        else if (nodeType == typeof(SetAllyNode))
        {
            var setAllyNode = CreateNode(canvas, SetAllyNode.ID) as SetAllyNode;
            allyBundle.prefabs.TryGetValue(args[1], out setAllyNode.prefab);
            setAllyNode.allyData = GetCharacterData(args[2]);
            setAllyNode.show = args[3] == "true";
            if (args.Length == 5)
            {
                setAllyNode.expr = args[4];
            }
            else if (args.Length >= 6)
            {
                setAllyNode.pose = args[4];
                setAllyNode.expr = args[5];
            }
            nodes.Add(setAllyNode);
        }
        else if (nodeType == typeof(AddEquippedSpellsNode))
        {
            var addSpellsNode = CreateNode(canvas, AddEquippedSpellsNode.Id) as AddEquippedSpellsNode;
            if (args.Length < 2)
            {
                throw new System.Exception($"Incorrect number of args for add spell node ({args.Length - 1}). Expected at least 2");
            }
            TryParseSpellWord(args[1], out addSpellsNode.word1, AddEquippedSpellsNode.Id);
            if (args.Length >= 3)
            {
                TryParseSpellWord(args[2], out addSpellsNode.word2, AddEquippedSpellsNode.Id);
            }
            if (args.Length >= 4)
            {
                TryParseSpellWord(args[3], out addSpellsNode.word3, AddEquippedSpellsNode.Id);
            }
            nodes.Add(addSpellsNode);
        }
        else if (nodeType == typeof(PauseNode))
        {
            var waitNode = CreateNode(canvas, PauseNode.ID) as PauseNode;
            if (args.Length < 2)
            {
                throw new System.Exception($"Incorrect number of args for wait node ({args.Length - 1}). Expected at least 1");
            }
            if(float.TryParse(args[1], out float fixedDuration))
            {
                waitNode.duration = fixedDuration;
            }
            else if(PauseNode.variables.ContainsKey(args[1]))
            {
                waitNode.variableName = args[1];
            }
            else
            {
                throw new System.Exception($"Invalid wait node args. {args[1]} is not a number or a valid variable");
            }

            nodes.Add(waitNode);
        }
        else
        {
            var gnode = CreateNodeGeneric(canvas, nodeType);
            if (gnode != null)
            {
                nodes.Add(gnode);
            }
        }
        return nodes;
    }



    // Parses a single dialog line. Returns constructed nodes.
    List<Node> ParseDialogNode(NodeCanvas canvas, string line)
    {
        int nameMarkerIndex = FindNameSeparatorIndex(line);
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
                        var hnode = CreateNode(canvas, SetPose.ID) as SetPose;
                        hnode.characterData = cds[i];
                        hnode.pose = poses[i];
                        nodes.Add(hnode);
                    }
                    if(!string.IsNullOrEmpty(exprs[i]))
                    {
                        var gnode = CreateNode(canvas, SetExpression.ID) as SetExpression;
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
                DialogNodeInput inode = CreateNode(canvas, DialogNodeInput.ID) as DialogNodeInput;
                inode.showChoicePrompt = carr.Length > 1;
                inode.variableName = carr[0];
                for (int i = 1; i < carr.Length; i++)
                    inode.choicePromptText[i - 1] = carr[i];
                dnode = inode;
            }
            else if(currView == typeof(DialogNodeVN))
            {
                dnode = CreateNode(canvas, DialogNodeVN.ID) as DialogNodeVN;
            }
            else
            {
                dnode = CreateNode(canvas, DialogNodeVNPlus.ID) as DialogNodeVNPlus;
            }
        }
        else if (currView == typeof(DialogViewChat))
        {
            // USE EXPRESSION TO CHANGE ICON (currently only 1 icon)
            var chatNode = CreateNode(canvas, DialogNodeChat.ID) as DialogNodeChat;
            if (poses.Count <= 0)
            {
                chatNode.iconSide = IconSide.NONE;
            }
            else if (poses[0] == "left")
            {
                chatNode.iconSide = IconSide.LEFT;
            }
            else if(poses[0] == "right")
            {
                chatNode.iconSide = IconSide.RIGHT;
            }
            else
            {
                chatNode.iconSide = IconSide.NONE;
            }
            if(exprs.Count > 0)
            {
                chatNode.timeText = exprs[0];
            }
            else
            {
                chatNode.timeText = string.Empty;
            }
            dnode = chatNode;
        }
        else if (currView == typeof(DialogViewAN))
        {
            var dnodeAN = CreateNode(canvas, DialogNodeAN.ID) as DialogNodeAN;

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
            var bubbleNode = CreateNode(canvas, DialogNodeBubble.ID) as DialogNodeBubble;
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
                        var setExprNode = CreateNode(canvas, SetExpression.ID) as SetExpression;
                        setExprNode.characterData = cds[0];
                        setExprNode.expr = exprs[0];
                        nodes.Add(setExprNode);
                    }
                    else if (exprData.Length == 2)
                    {
                        // Pose + expression
                        var setPoseNode = CreateNode(canvas, SetPose.ID) as SetPose;
                        setPoseNode.characterData = cds[0];
                        setPoseNode.pose = exprData[0];
                        nodes.Add(setPoseNode);

                        // (Optional) expression change
                        if (!string.IsNullOrWhiteSpace(exprData[1]))
                        {
                            var setExprNote = CreateNode(canvas, SetExpression.ID) as SetExpression;
                            setExprNote.characterData = cds[0];
                            setExprNote.expr = exprData[1];
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
        else if (currView == typeof(DialogViewLocation))
        {
            dnode = CreateNode(canvas, DialogNodeLocation.ID) as DialogNodeLocation;
        }
        dnode.characterName = cname.Substring(0, cname.Length-1);
        dnode.displayName = displayName;
        dnode.text = dialogLine[1].Trim().Replace("…", "...").Replace('’', '\'');
        nodes.Add(dnode);
        return nodes;
    }

    private int FindNameSeparatorIndex(string line)
    {
        bool inExpr = false;
        for (int i = 0; i < line.Length; ++i)
        {
            if (line[i] == '(')
            {
                inExpr = true;
            }
            else if (inExpr)
            {
                if (line[i] == ')')
                {
                    inExpr = false;
                }
            }
            else if (line[i] == ':')
            {
                return i;
            }
        }
        return -1;
    }

    // Get character data asset from alias (null if character doesn't exist).
    CharacterData GetCharacterData(string alias)
    {
        if (!allCharacterData.Any(c => c.aliases.Contains(alias))) return null;
        var cdata = allCharacterData.Single(c => c.aliases.Contains(alias)); // Find character data.
        return AssetDatabase.LoadAssetAtPath<CharacterData>("Assets/ScriptableObjects/CharacterData/" + cdata.name + ".asset");
    }

    // Creates node and shifts position.
    Node CreateNode(NodeCanvas canvas, string id)
    {
        var gnode = Node.Create(id, Vector2.right * pos, canvas);
        pos += gnode.MinSize.x + nodeSpacing;
        return gnode;
    }

    private Node CreateNodeGeneric(NodeCanvas canvas, System.Type nodeType)
    {
        if(!nodeIDMap.TryGetValue(nodeType, out var id))
        {
            return null;
        }
        return CreateNode(canvas, id);
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

    private bool TryParseFadeCurve(string curveType, float time, bool isFadeIn, out AnimationCurve curve)
    {
        float start = isFadeIn ? 0 : 1;
        float end = isFadeIn ? 1 : 0;
        string curveTypeProcessed = curveType.ToLower();
        if (curveTypeProcessed == "ease")
        {
            curve = AnimationCurve.EaseInOut(0, start, time, end);
            return true;
        }
        if (curveTypeProcessed == "linear")
        {
            curve = AnimationCurve.Linear(0, start, time, end);
            return true;
        }
        if (curveTypeProcessed == "constant" || curveTypeProcessed == "immediate")
        {
            curve = AnimationCurve.Constant(0, 0, end);
            return true;
        }
        if (customFadeCurves.ContainsKey(curveTypeProcessed))
        {
            curve = customFadeCurves[curveTypeProcessed];
            return true;
        }
        curve = null;
        return false;
    }

    private bool TryParseFadeCurve(string curveType, string time, bool isFadeIn, out AnimationCurve curve)
    {
        if(!float.TryParse(time, out float timeNumber))
        {
            curve = null;
            return false;
        }
        return TryParseFadeCurve(curveType, timeNumber, isFadeIn, out curve);
    }

    private T LoadAsset<T>(string assetName, string folderPath) where T : UnityEngine.Object
    {
        string path = AssetDatabase.FindAssets(assetName, AssetDatabase.GetSubFolders(folderPath))[0];
        path = AssetDatabase.GUIDToAssetPath(path);
        return AssetDatabase.LoadAssetAtPath<T>(path);
    }
}
