using UnityEngine;
using System.Collections;

public class DotUwsDoorCloser : StateMachineBehaviour {
	
	private const float _shiftY = 3.48f;
	
	private Transform _slider = null;
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (_slider == null) {
			_slider = animator.transform.parent.Find("Door_Slider");
		}
		if (_slider != null) {
			Vector3 _pos = _slider.localPosition;
			_pos.y = (Mathf.Abs(_pos.y) < _shiftY / 2.0) ? 0.0f : -_shiftY;
			_slider.localPosition = _pos;
		}
	}
	
}
