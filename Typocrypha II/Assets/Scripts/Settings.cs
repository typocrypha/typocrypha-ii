using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class Settings
{
    private const string gameSpeedKey = "gameSpeed";
    private const float gameSpeedDefault = 1;
    private const string uiSpeedKey = "uiSpeed";
    private const float uiSpeedDefault = 1;
    private const string textScrollSpeedKey = "textScrollSpeed";
    private const float textScrollSpeedDefault = 1;
    private const string keyLayoutKey = "keyLayout";
    private const KeyLayoutType keyLayoutDefault = KeyLayoutType.QWERTY;
    private const string customKeyLayoutKey = "customKeyLayout";
    private const string customKeyLayoutDefault = Typocrypha.KeyboardBuilder.keyboardFormatQwerty;
    private const string autoContinueKey = "autoContinue";
    private const bool autoContinueDefault = false;

    public enum KeyLayoutType
    {
        CUSTOM,
        QWERTY,
        DVORAK,
        COLEMAK
    }

    public static KeyLayoutType KeyLayout
    {
        get
        {
            SafeInitialize();
            return (KeyLayoutType)PlayerPrefs.GetInt(customKeyLayoutKey);
        }
        set
        {
            PlayerPrefs.SetInt(customKeyLayoutKey, (int)value);
        }
    }

    public static string CustomKeyLayout
    {
        get
        {
            SafeInitialize();
            return PlayerPrefs.GetString(customKeyLayoutKey);
        }
        set
        {
            PlayerPrefs.SetString(customKeyLayoutKey, value);
        }
    }

    public static float GameplaySpeed
    {
        get
        {
            SafeInitialize();
            return gameSpeed;
        }
        set
        {
            gameSpeed = value;
            PlayerPrefs.SetFloat(gameSpeedKey, value);
        }
    }
    private static float gameSpeed = gameSpeedDefault;

    public static float UISpeed
    {
        get
        {
            SafeInitialize();
            return uiSpeed;
        }
        set
        {
            uiSpeed = value;
            PlayerPrefs.SetFloat(uiSpeedKey, value);
        }
    }
    private static float uiSpeed = uiSpeedDefault;

    const float baseScrollDelay = 0.028f; // Default text scrolling speed.
    public static float TextScrollSpeed
    {
        get
        {
            SafeInitialize();
            return textScrollSpeed;
        }
        set
        {
            textScrollSpeed = value;
            Time.fixedDeltaTime = baseScrollDelay / textScrollSpeed;
            PlayerPrefs.SetFloat(textScrollSpeedKey, value);
        }
    }
    private static float textScrollSpeed = textScrollSpeedDefault;

    public static bool AutoContinue
    {
        get
        {
            SafeInitialize();
            return autoContinue;
        }
        set
        {
            autoContinue = value;
            PlayerPrefs.SetInt(autoContinueKey, value ? 1 : 0);
        }
    }
    private static bool autoContinue = autoContinueDefault;

    private static bool initialized = false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SafeInitialize()
    {
        if(!initialized)
        {
            Initialize();
            initialized = true;
        }
    }

    public static void Initialize()
    {
        if(!PlayerPrefs.HasKey(gameSpeedKey))
        {
            GameplaySpeed = gameSpeedDefault;
        }
        else
        {
            gameSpeed = PlayerPrefs.GetFloat(gameSpeedKey);
        }
        if (!PlayerPrefs.HasKey(uiSpeedKey))
        {
            UISpeed = uiSpeedDefault;
        }
        else
        {
            uiSpeed = PlayerPrefs.GetFloat(uiSpeedKey);
        }
        if (!PlayerPrefs.HasKey(textScrollSpeedKey))
        {
            TextScrollSpeed = textScrollSpeedDefault;
        }
        else
        {
            textScrollSpeed = PlayerPrefs.GetFloat(textScrollSpeedKey);
        }
        if (!PlayerPrefs.HasKey(keyLayoutKey))
        {
            KeyLayout = keyLayoutDefault;
        }
        if (!PlayerPrefs.HasKey(customKeyLayoutKey))
        {
            CustomKeyLayout = customKeyLayoutDefault;
        }
        if (!PlayerPrefs.HasKey(autoContinueKey))
        {
            AutoContinue = autoContinueDefault;
        }
        else
        {
            autoContinue = PlayerPrefs.GetInt(autoContinueKey) > 0;
        }
    }
}
