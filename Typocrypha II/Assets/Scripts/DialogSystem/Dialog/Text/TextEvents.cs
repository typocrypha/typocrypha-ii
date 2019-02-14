using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate IEnumerator TextEventDel(string[] opt);

/// <summary>
/// Struct to contain a text event.
/// </summary>
public struct TextEvent
{ 
	public string evt; // Name of event
	public string[] opt; // Options of event
    public int pos; // Position in text of event
	public TextEvent(string evt, string[] opt, int pos)
    {
		this.evt = evt;
		this.opt = opt;
        this.pos = pos;
	}
}

/// <summary>
/// Event class for events during text dialogue.
/// Individual events are Coroutine handles.
/// Events should handle by themselves the case where dialog is skipped mid scroll.
/// </summary>
public class TextEvents : MonoBehaviour
{
	public static TextEvents instance = null;
	public Dictionary<string, TextEventDel> textEventMap; // Map of commands to text event handles.

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
        
        textEventMap = new Dictionary<string, TextEventDel>
        {
            {"pause", Pause},
            {"shake", ScreenShake }
        };
    }

    /// <summary>
    /// Plays a text event.
    /// </summary>
    /// <param name="evt">Name of event.</param>
    /// <param name="opt">Parameters to event.</param>
    /// <returns>Coroutine of event (null if none).</returns>
    public Coroutine PlayEvent(string evt, string[] opt)
    {
        if (!textEventMap.TryGetValue(evt, out TextEventDel textEvent))
        {
            Debug.LogException(new System.Exception("Bad text event parameters:" + evt));
        }
        Coroutine cr = StartCoroutine(textEvent(opt));
        return cr;
    }

    ///**************************** TEXT EVENTS *****************************/

    /// <summary>
    /// Pauses text scroll for a fixed amount of time.
    /// Automatically unpauses if text scroll is done.
    /// </summary>
    /// <param name="opt">
    /// [0]:float: Length of pause.
    /// </param>
    IEnumerator Pause(string[] opt)
    {
        DialogManager.instance.dialogBox.Pause = true;
        float time = 0f;
        float endTime = float.Parse(opt[0]);
        while (time < endTime && !DialogManager.instance.dialogBox.IsDone)
        {
            yield return new WaitForFixedUpdate();
            time += Time.fixedDeltaTime;
        }
        DialogManager.instance.dialogBox.Pause = false;
    }

    /// <summary>
    /// Shakes the screen.
    /// </summary>
    /// <param name="opt">
    /// [0]: float, intensity of shake.
    /// [1]: float, length of shake.
    /// </param>
    IEnumerator ScreenShake(string[] opt)
    {
        Coroutine shake = CameraManager.instance.Shake(float.Parse(opt[0]), float.Parse(opt[1]));
        yield return new WaitUntil(() => DialogManager.instance.dialogBox.IsDone);
        CameraManager.instance.ResetCamera();
        yield return true;
    }

    //	// fades the screen in or out
    //	// input: [0]: [in|out], 'in' re-reveals screen, 'out' hides it
    //	//        [1]: float, length of fade in seconds
    //	//        [2]: float, red color component
    //	//        [3]: float, green color component
    //	//        [4]: float, blue color component
    //	IEnumerator fade(string[] opt) {
    //		float fade_time = float.Parse (opt [1]);
    //		float r = float.Parse (opt [2]);
    //		float g = float.Parse (opt [3]);
    //		float b = float.Parse (opt [4]);
    //		float alpha;
    //		float a_step = Time.deltaTime / fade_time; // amount of change each frame
    //        if (a_step > 1) a_step = 1;
    //		alpha = dimmer.color.a;
    //		if (opt [0].CompareTo ("out") == 0) { // hide screen
    //			while (alpha < 1f) {
    //				yield return null;
    //                a_step = Time.deltaTime / fade_time;
    //                alpha += a_step;
    //				dimmer.color = new Color (r, g, b, alpha);
    //			}
    //		} else { // show screen
    //			while (alpha > 0f) {
    //				yield return null;
    //                a_step = Time.deltaTime / fade_time;
    //                alpha -= a_step;
    //				dimmer.color = new Color (r, g, b, alpha);
    //			}
    //		}
    //	}

    //	// NON_OPERATIONAL
    //	// scrolls floating text center aligned in the center of the screen (can also be used to immediately show text)
    //	// input: [0]: float, delay time in seconds
    //	//        [1]: float, red color
    //	//        [2]: float, green color
    //	//        [3]: float, blue color
    //	//        [4]: string, text to show
    //	IEnumerator centerTextScroll(string[] opt) {
    //		Text txt = center_text.GetComponent<Text> ();
    //		float delay = float.Parse (opt [0]);
    //		float r = float.Parse (opt [1]);
    //		float g = float.Parse (opt [2]);
    //		float b = float.Parse (opt [3]);
    //		float a = txt.color.a;
    //		string txt_str = opt [4];
    //		txt.color = new Color (r, g, b, a);
    //		if (delay == 0) { // show text immediately
    //			txt.text = txt_str;
    //		} else { // scroll text character by character
    //			txt.text = "";
    //			int txt_pos = 0;
    //			while (txt_pos < txt_str.Length) {
    //				txt.text += txt_str [txt_pos++];
    //				yield return new WaitForSeconds (delay);
    //			}
    //		}
    //	}

    //	// NON_OPERATIONAL
    //	// fades center text in or out
    //	// input: [0]: [in|out], 'in' re-reveals screen, 'out' hides it
    //	//        [1]: float, length of fade in seconds
    //	IEnumerator centerTextFade(string[] opt) {
    //		Text txt = center_text.GetComponent<Text> ();
    //		float fade_time = float.Parse (opt [1]);
    //		float r = txt.color.r;
    //		float g = txt.color.g;
    //		float b = txt.color.b;
    //		float alpha = txt.color.a;
    //		float a_step = 1f * 0.017f / fade_time; // amount of change each frame
    //		if (a_step > 1) a_step = 1;
    //		if (opt [0].CompareTo ("out") == 0) { // hide text
    //			while (alpha > 0f) {
    //				yield return new WaitForSeconds(0.017f);
    //				alpha -= a_step;
    //				txt.color = new Color (r, g, b, alpha);
    //			}
    //		} else { // show screen
    //			while (alpha < 1f) {
    //				yield return new WaitForSeconds(0.017f);
    //				alpha += a_step;
    //				txt.color = new Color (r, g, b, alpha);
    //			}
    //		}
    //	}

    //	// plays the specified sfx
    //	// input: [0]: string, sfx filename
    //	IEnumerator playSFX(string[] opt) {
    //		AudioPlayer.main.playSFX (opt[0]);
    //		yield return true;
    //	}

    //	// plays specified music
    //	// input: [0]: string, music filename
    //	IEnumerator playMusic(string[] opt) {
    //		AudioPlayer.main.playMusic (opt [0]);
    //		yield return true;
    //	}

    //	// stops music
    //	IEnumerator stopMusic(string[] opt){
    //		AudioPlayer.main.stopMusic ();
    //		yield return true;
    //	}

    //	// fades music in/out (linear)
    //	// input: [0]: string, music filename (if "_", no change)
    //	//        [1]: [in|out], if 'in', fades music in, if 'out' fades music out
    //	//        [2]: float, time (in seconds) over which to transition
    //	IEnumerator fadeMusic(string[] opt) {
    //		if (opt [0] != "_")
    //			AudioPlayer.main.playMusic (opt [0]);
    //		bool dir = opt [1] == "out" ? true : false;
    //		AudioPlayer.main.fadeMusic (dir, float.Parse (opt [2]));
    //		yield return true;
    //	}

    //	// set scroll delay scale
    //	// input: [0]: float, new scroll scale amount
    //	IEnumerator setScrollScale(string[] opt) {
    //		DialogBox.ScrollScale = float.Parse (opt [0]);
    //		yield return true;
    //	}

    //	// sets background image from sprite name
    //	// input: [0]: string, name of image file
    //	IEnumerator setBG(string[] opt) {
    //		BackgroundEffects.main.setSpriteBG (opt [0]);
    //		yield return true;
    //	}

    //	// hides/shows dialogue box (NOTE: text is STILL GOING when hidden)
    //	// typically, should block when hiding to avoid skipping reshow event
    //	// input: [0]: [t|n], hides text box if 't', shows if 'f'
    //	IEnumerator hideTextBox(string[] opt) {
    //		dialogue_box.SetActive (opt[0].CompareTo("f") == 0);
    //		yield return true;
    //	}

    //	// sets the talking sfx
    //	// input: [0]: string, name of audio file
    //	IEnumerator setTalkSFX(string[] opt) {
    //		AudioPlayer.main.setSFX (3, opt[0]); // put sfx in channel 3
    //		yield return true;
    //	}

    //	// Allows for highlighting a character
    //	// input: [0]: string, name of sprite to highlight
    //	//        [1]: float, amount to highlight (multiplier to tint)
    //	IEnumerator highlightCharacter(string[] opt) {
    //        //DialogueManager.main.highlightCharacter(opt[0], float.Parse(opt[1]));
    //        throw new System.NotImplementedException("Obsolete unless integrated with charactermanager");
    //		yield return true;
    //	}

    //	// Highlights one character and unhighlights all others (0.5 greyscale)
    //	// input: [0]: string, name of sprite to highlight
    //	IEnumerator soleHighlight(string[] opt) {
    //        //DialogueManager.main.soleHighlight (opt [0]);
    //        throw new System.NotImplementedException("Obsolete unless integrated with charactermanager");
    //        yield return true;
    //	}

    //    // Highlights one character and unhighlights all others (0.5 greyscale)
    //    // input: [0]: string, name of sprite to highlight
    //    IEnumerator soleHighlightCodec(string[] opt)
    //    {
    //        //DialogueManager.main.soleHighlightCodec();
    //        throw new System.NotImplementedException("Obsolete unless integrated with charactermanager");
    //        yield return true;
    //    }

    //    // Removes a specific character from the scene
    //    // input: [0]: string, name of sprite to remove
    //    IEnumerator removeCharacter(string[] opt) {
    //        //DialogueManager.main.removeCharacter (opt [0]);
    //        throw new System.NotImplementedException("Obsolete unless integrated with charactermanager");
    //        yield return true;
    //	}

    //	// Removes all characters from a scene
    //	// input: NONE
    //	IEnumerator removeAllCharacter(string[] opt) {
    //        //DialogueManager.main.removeAllCharacter ();
    //        throw new System.NotImplementedException("Obsolete unless integrated with charactermanager");
    //        yield return true;
    //	}

    //	// NON_OPERATIONAL
    //	IEnumerator glitch(string[] opt)
    //	{
    //		for(int i = 0; i < 5; i++)
    //		{
    //			glitch_effect.SetActive(true);
    //			yield return new WaitForSeconds (0.05f);
    //			glitch_effect.SetActive(false);
    //			yield return new WaitForSeconds (0.05f);
    //		}
    //		glitch_effect.SetActive(true);
    //		yield return new WaitForSeconds (0.15f);
    //		glitch_effect.SetActive(false);

    //		yield return true;
    //	}

    //	// Sets name from last inputed text
    //	// Input: NONE
    //	IEnumerator setName(string[] opt) {
    //		PlayerDataManager.main.PlayerName = PlayerDataManager.main.LastPlayerInput;
    //		yield return true;
    //	}

    //    // Sets specified info from last inputed text
    //    // Input: [0]: string: the key of the info to set (from input if only one arg)
    //    //        [1]: string: the data to set the info to (optional)
    //    IEnumerator setInfo(string[] opt)
    //    {
    //        if (opt.Length == 1)
    //            PlayerDataManager.main.setData(opt[0], PlayerDataManager.main.LastPlayerInput);
    //        else if (opt.Length == 2)
    //            PlayerDataManager.main.setData(opt[0], opt[1]);
    //        else
    //            throw new System.Exception("TextEvents.cs: setInfo: Too many args, use 1 or 2");
    //        yield return true;
    //    }

    //    // makes the screenframe come in/out
    //    // input: [0]: [in|out], 'in' re-reveals screenframe, 'out' hides it
    //    IEnumerator frame(string[] opt) {
    //		if (opt [0].CompareTo ("out") == 0) { // hide screenframe
    //			screen_frame.SetActive(false);
    //		} else { // show screenframe
    //			screen_frame.SetActive(true);
    //		}
    //		yield return true;
    //	}

    //	// restore player to full HP
    //	// input: N/A
    //	IEnumerator healPlayer(string[] opt) {
    //        throw new System.NotImplementedException();
    //		//Player.main.restoreToFull ();
    //        BattleManagerS.main.battleKeyboard.clearStatus();
    //		yield return true;
    //	}

    //	// clears the chat and AN text logs
    //	// input: [0]: [chat|an], 'chat' clears chat log, 'an' clears AN log
    //	IEnumerator clearTextLog(string[] opt){
    //        //if (opt [0].CompareTo ("chat") == 0) {
    //        //	Debug.Log ("chat log flushed");
    //        //	DialogueManager.main.clearChatLog();
    //        //}
    //        //else if (opt [0].CompareTo ("an") == 0){
    //        //	Debug.Log ("an log flushed");
    //        //	DialogueManager.main.clearANLog();
    //        //}
    //        throw new System.NotImplementedException("Obsolete unless integrated with dialogManager, should be deprecated");
    //        yield return true;
    //	}

    //	// scrolls floating text at a specific location
    //	// input: [0]: float, x-position in world coordinates (bottom left corner)
    //	//        [1]: float, y-position in world coordinates
    //	//        [2]: string, text to be displayed
    //	IEnumerator floatText(string[] opt) {
    //		// setup dialogue item and dialogue box
    //		GameObject d_box_obj = Instantiate (float_d_box_prefab, float_text_view);
    //		string text = opt [2];
    //		for (int i = 3; i < opt.Length; i++) text += "," + opt [i];
    //		d_box_obj.GetComponent<FloatText> ().startFloatText (float.Parse(opt[0]), float.Parse(opt[1]), text);
    //		yield return true;
    //	}

    //	// scrolls a bunch of floating text objects at random locations
    //	// input: [0]: int, number of floating text objs
    //	//        [1]: string, text to be display
    //	IEnumerator multiFloatText(string[] opt) {
    //		int num = int.Parse (opt [0]);
    //		for (int i = 0; i < num; i++) {
    //			// setup dialogue item and dialogue box
    //			GameObject d_box_obj = Instantiate (float_d_box_prefab, float_text_view);
    //			string text = opt [1];
    //			for (int j = 3; j < opt.Length; j++) text += "," + opt [j];
    //			float x = ((Random.value - 0.5f) * 18) - 2;
    //			float y = (Random.value - 0.5f) * 10;
    //			d_box_obj.GetComponent<FloatText> ().startFloatText (x, y, text);
    //			yield return null;
    //		}
    //	}

    //	// plays train transition animation
    //	// input: [0]: [in|out], 'in' shows train scene, 'out' hides it
    //	IEnumerator trainTransition(string[] opt) {
    //		if (opt[0] == "in") 
    //			train_animator.Play ("train_fade_in");
    //		else
    //			train_animator.Play ("train_fade_out");
    //		yield return true;
    //	}

    //	// switches train transition sprite
    //	// input: [0]: string, name of sprite
    //	IEnumerator trainSwitch(string[] opt) {
    //		//Sprite spr = train_bundle.LoadAsset<Sprite>(opt[0]);
    //		int num = int.Parse (opt [0]);
    //		train_sprite.sprite = trainsition_sprites[num];
    //		yield return true;
    //	}

    //	// plays train animation where character wiggles their flag
    //	// input: [0]: string, character's name
    //	IEnumerator trainSign(string[] opt) {
    //		train_animator.Play ("train_" + opt[0] + "_sign");
    //		yield return true;
    //	}

    //	// moves train character sprites
    //	// input: [0]: string, name of character's game object
    //	//        [1]: int, 0-11, which seat (from leftmost complete seat) to move character
    //	IEnumerator trainChrPos(string[] opt) {
    //		Transform chr = train_sprite.transform.Find (opt[0]);
    //		chr.localPosition = new Vector2 ((float.Parse (opt [1]) * 0.7825f) - 4.199f, chr.localPosition.y);
    //		yield return true;
    //	}

    //	// changes evil eye frame emotion animation
    //	// input: [0]: string, name of expression to play
    //	IEnumerator eyeEmote(string[] opt) {
    //		string animName = "anim_evil_eye_frame_" + (opt[0]);
    //		eye_frame_animator.Play (animName);
    //		yield return true;
    //	}

    //	// changes train transition display text and plays animation of text appearing
    //	// input: [0]: string, 1st line (Location)
    //	// input: [1]: string, 2nd line (Time/etc.)
    //	IEnumerator trainTextShow(string[] opt) {
    //		string newText = "<size=28>NEXT DESTINATION:</size>\n" + (opt[0]) + "\n>> " + (opt[1]);
    //		train_text.text = newText;
    //		train_text_animator.Play ("train_destination_text");
    //		yield return true;
    //	}

    //	// Resets camera animator
    //	// input: NONE
    //	IEnumerator resetCamera(string[] opt) {
    //		AnimationPlayer.main.playScreenEffect ("NULL");
    //		yield return true;
    //	}

    //	// Shows credits object
    //	// input: NONE
    //	IEnumerator credits(string[] opt) {
    //		if(credits_obj.activeSelf == false ) credits_obj.SetActive (true);
    //		else credits_obj.SetActive (false);
    //		yield return true;
    //	}

    //	// Plays credits video
    //	// input: NONE
    //	IEnumerator creditsPlay(string[] opt) {
    //		credits_video.Play ();
    //		//yield return new WaitUntil (credits_video.loopPointReached);
    //		yield return true;
    //	}

    //	// Shows stinger credits object
    //	// input: NONE
    //	IEnumerator creditsStinger(string[] opt) {
    //		if(credits_stinger_obj.activeSelf == false ) credits_stinger_obj.SetActive (true);
    //		else credits_stinger_obj.SetActive (false);
    //		yield return true;
    //	}

    //	IEnumerator quitGame(string[] opt) {
    //		Application.Quit ();
    //		yield return true;
    //	}

    //	IEnumerator blockPause(string[] opt) {
    //		if (opt [0].CompareTo ("t") == 0)
    //			pause_menu.block_pause = true;
    //		else pause_menu.block_pause = false;
    //		yield return true;
    //	}
}

