public var turnSpeed : float = 50f;

function Start () {

}

function Update () {
	if(Input.GetKey(KeyCode.LeftArrow)){
		transform.Rotate(Vector3.down, turnSpeed * Time.deltaTime);
	}
	if(Input.GetKey(KeyCode.RightArrow)){
		transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
	}
	if(Input.GetKey(KeyCode.UpArrow)){
		transform.Rotate(Vector3.left, turnSpeed * Time.deltaTime);
	}
	if(Input.GetKey(KeyCode.DownArrow)){
		transform.Rotate(Vector3.right, turnSpeed * Time.deltaTime);
	}
}