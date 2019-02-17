using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTEST : MonoBehaviour
{
    public CharacterData data;
    public CharacterData data2;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        DialogCharacterManager.instance.AddCharacter(data, "base", "normal", Vector2.zero);
        yield return new WaitForSeconds(2f);
        DialogCharacterManager.instance.RemoveCharacter(data2);
        yield return new WaitForSeconds(2f);
    }

}
