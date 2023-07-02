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
            SafeIntialize();
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
            SafeIntialize();
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
            SafeIntialize();
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
            SafeIntialize();
            return uiSpeed;
        }
        set
        {
            uiSpeed = value;
            PlayerPrefs.SetFloat(uiSpeedKey, value);
        }
    }
    private static float uiSpeed = uiSpeedDefault;

    public static float TextScrollSpeed
    {
        get
        {
            SafeIntialize();
            return textScrollSpeed;
        }
        set
        {
            textScrollSpeed = value;
            PlayerPrefs.SetFloat(textScrollSpeedKey, value);
        }
    }
    private static float textScrollSpeed = textScrollSpeedDefault;

    private static bool initialized = false;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SafeIntialize()
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
    }
}
