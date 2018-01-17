using UnityEngine;
using UnityEngine.SceneManagement;

public class KeepObject : MonoBehaviour {

    private void Awake() {
        DontDestroyOnLoad(gameObject); //keep music going into scenes
    }

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }
}
