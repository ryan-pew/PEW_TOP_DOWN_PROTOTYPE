// In order to the doors (as for prefab Door) automatically trigger when approaching a character,
// object FirstPersonСontroller should be marked with the tag "Player"

using UnityEngine;
using System.Collections;

public class DotUwsDoorSlide : MonoBehaviour {

	private Animator _animator = null;
	private AudioSource _doorSnd = null;
	private AudioClip[] _sounds = new AudioClip[2];
	private bool _sndLoaded = false;
	private int _plaingSnd = -1; // 0-Open, 1-Close 

	void Start () {
		foreach(Transform child in transform.parent.transform){
			switch(child.name){
			case "Door_Slider": _animator = child.GetComponent<Animator>(); break;
			case "Door_Sound": _doorSnd  = child.GetComponent<AudioSource>(); break;
			}
		};
		_sounds[0] = Resources.Load("Open_Sound") as AudioClip;
		_sounds[1] = Resources.Load("Close_Sound") as AudioClip;
		_sndLoaded = (_sounds[0]!=null) && (_sounds[1] != null);
		if (!_sndLoaded) {
			Debug.LogWarning("Silence mode:  audioclips \"Open_Sound\" and / or \"Close_Sound\" not found in the \"Resources\" directory");
		}
	}
	
 	void slide_door(Collider other, int _id){ // 0 - Open, 1 - Close
		string _anim = "Door_"+((_id == 0) ? "Open" : "Close");
		if( ( _animator != null) && (other.gameObject.tag == "Player") ){
			AnimatorStateInfo _st = _animator.GetCurrentAnimatorStateInfo(0);
			if( !_st.IsName(_anim) ){
				float _time = _st.normalizedTime;
				_time = (_time<1.0f && (_st.IsName("Door_Open") || _st.IsName("Door_Close"))) ? 1 - _time : 0.0f;
				if( _sndLoaded ){
					float _timeSnd = 0.0f;
					if( _doorSnd.isPlaying && (_id > 0) && (_plaingSnd != _id) ){
						_timeSnd = _sounds[_id].length - _doorSnd.time;
					}
					_doorSnd.clip = _sounds[_id];
					_doorSnd.time = _timeSnd;
					_plaingSnd = _id;
					_doorSnd.Play();
				}
				_animator.Play(_anim, -1, _time );
			}
		}
	}

	void OnTriggerEnter(Collider other){
		slide_door(other, 0); // Open door
	}	

	void OnTriggerExit(Collider other){
		slide_door(other, 1); // Close door
	}	

}