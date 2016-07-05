var speed = Vector3(10,20,30);

function Update () {
	transform.Rotate (speed * Time.deltaTime);
}