using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

//Made By Leon
public class CameraAroundWithDragMove : MonoBehaviour {

	public static CameraAroundWithDragMove instance;

	public float aroundInertia=2;
	public Vector2 aroundMouseSpeed=new Vector2(12,12);
	public int aroundCurrentID=4;
	public Vector2[] aroundViewPoints={new Vector2(10,20f),new Vector2(30,100f),new Vector2(45,200f),new Vector2(60,400f),new Vector2(75,600f),new Vector2(90,1000f)};
	public Vector2 mouseYLimit = new Vector2 (20,80);
	public Collider dragArea;

	private Camera myCamera;
	private float aroundAutoSpeed=0;
	private Transform targetObject;
	private float distance,targetDistance,timeCount;
	private float lastMouseX,rotateDirectionX,lastMouseY,rotateDirectionY;
	private Vector2 currentMouse,targetRot;
	private Vector3 targetCenterPos,mousePos_DragStart,targetWorldPos_DragStart;

	void Awake () 
	{
		instance = this;

		//Get Camera Component
		myCamera=GetComponent<Camera>();
		//Create Center
		targetObject=GameObject.CreatePrimitive(PrimitiveType.Cube).transform;
		Destroy(targetObject.GetComponent<Collider>());
		Destroy(targetObject.GetComponent<Renderer>());
		Destroy(targetObject.GetComponent<MeshFilter>());
		targetObject.name="Center";
		//-----------------------------------------
		currentMouse.x = transform.eulerAngles.y;
		currentMouse.y = transform.eulerAngles.x;
		
		if(aroundInertia<=0)   //will output error
			aroundInertia=0.01f;
		//-----------------------------------------
		Format();
	}
	
	void Update () 
	{
//----------------------------------------------Mouse Control
		if (Input.GetMouseButtonDown (1) && !EventSystem.current.IsPointerOverGameObject ()) {
			mousePos_DragStart = Input.mousePosition;
			targetWorldPos_DragStart = targetCenterPos;

			targetRot = currentMouse;
		}
		if (Input.GetMouseButton (1) && !EventSystem.current.IsPointerOverGameObject ()) {
			
			Vector3 mouseDragPosOffset = Input.mousePosition - mousePos_DragStart;
			Vector3 targetScreenPos = myCamera.WorldToScreenPoint (targetWorldPos_DragStart) - mouseDragPosOffset;

			Ray theRay = myCamera.ScreenPointToRay (targetScreenPos);
			RaycastHit[] hits = Physics.RaycastAll (theRay);
			for (int i = 0; i < hits.Length; i++) {
				if (hits [i].collider == dragArea) {
					targetCenterPos = hits [i].point;
					timeCount = 0.618f;
					break;
				}
			}
		}

		if (Input.GetMouseButtonDown (0) && !EventSystem.current.IsPointerOverGameObject ()) {
			timeCount = 1;
			if(Time.time<1)
				currentMouse = targetRot;
		}
		else if (timeCount < 1) {    //Lerp Speed
			timeCount += Time.deltaTime;
			currentMouse = Vector2.Lerp (currentMouse, targetRot, Mathf.Pow(timeCount,2));
		}

//--------------------------------------------------Around
	    if (targetObject)  
		{
			if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject ())               //shou dong xuan zhuan
			{
				//Debug.Log(EventSystem.current.IsPointerOverGameObject());


				float theXdelta=0,theYdelta=0;
				
				if(Input.GetAxis("Mouse X")>1)            //limit the X Axis
					theXdelta=1;
				else if(Input.GetAxis("Mouse X")<-1)
					theXdelta=-1;
				else
					theXdelta=Input.GetAxis("Mouse X");
				
				if(Input.GetAxis("Mouse Y")>1)           //limit the Y Axis
					theYdelta=1;
				else if(Input.GetAxis("Mouse Y")<-1)
					theYdelta=-1;
				else
					theYdelta=Input.GetAxis("Mouse Y");
		
				if(theXdelta>0&&theXdelta>lastMouseX||theXdelta<0&&theXdelta<lastMouseX)   //Only currentX>lastX ,we update the lastX
					lastMouseX=theXdelta; 
				if(theYdelta>0&&theYdelta>lastMouseY||theYdelta<0&&theYdelta<lastMouseY)	//Only currentY>lastY ,we update the lastY
					lastMouseY=theYdelta;
				
				if(Mathf.Abs(lastMouseX)<0.01f)          //Make X be zero
					lastMouseX=0;
				if(Mathf.Abs(lastMouseY)<0.01f)			//Make Y be zero
					lastMouseY=0;
					
				if(lastMouseX>0) 					//record X direction
					rotateDirectionX=1;
				if(lastMouseX<0)
					rotateDirectionX=-1;
				if(lastMouseX==0)
					rotateDirectionX=0;
				
				if(lastMouseY>0)					//record Y direction
					rotateDirectionY=1;
				if(lastMouseY<0)
					rotateDirectionY=-1;
				if(lastMouseY==0)
					rotateDirectionY=0;
	
				MoveWithInertia();
			}
			else if(aroundAutoSpeed!=0)         //auto
			{
				currentMouse.x += aroundAutoSpeed*0.2f ; 
				transform.rotation = Quaternion.Euler(currentMouse.y, currentMouse.x, 0);
				transform.position = Quaternion.Euler(currentMouse.y, currentMouse.x, 0) * new Vector3(0,0, -distance) + targetObject.position;
			}
			else                     //guan xing
				MoveWithInertia();
//--------------------------------------------------------------------------------Mouse Scroll Wheel
			if (Input.GetAxis ("Mouse ScrollWheel") > 0.09f && targetDistance > aroundViewPoints [0].y && !EventSystem.current.IsPointerOverGameObject ()) {    //Limit the Distance
				if (aroundCurrentID > 0) {
					aroundCurrentID--;
					targetDistance = aroundViewPoints [aroundCurrentID].y;
					targetRot = new Vector2 (currentMouse.x, aroundViewPoints [aroundCurrentID].x);
				} else {
					targetDistance = aroundViewPoints [0].y;
					targetRot = new Vector2 (currentMouse.x, aroundViewPoints [0].x);
				}
				timeCount = 0;
			}
			if (Input.GetAxis ("Mouse ScrollWheel") < -0.09f && targetDistance < aroundViewPoints [aroundViewPoints.Length - 1].y && !EventSystem.current.IsPointerOverGameObject ()) {
				if (aroundCurrentID < aroundViewPoints.Length - 1) {
					aroundCurrentID++;
					targetDistance = aroundViewPoints [aroundCurrentID].y;
					targetRot = new Vector2 (currentMouse.x, aroundViewPoints [aroundCurrentID].x);
				} else {
					targetDistance = aroundViewPoints [aroundViewPoints.Length - 1].y;
					targetRot = new Vector2 (currentMouse.x, aroundViewPoints [aroundViewPoints.Length - 1].x);
				}
				timeCount = 0;
			}
			if(targetDistance>aroundViewPoints[aroundViewPoints.Length-1].y)
				targetDistance=aroundViewPoints[aroundViewPoints.Length-1].y;
			if(targetDistance<aroundViewPoints[0].y)
				targetDistance=aroundViewPoints[0].y;
			distance=Vector2.Lerp(new Vector2(distance,0),new Vector2(targetDistance,0),Mathf.Pow(timeCount,2)).x; //Lerp the distance
//------------------------------------------------------------------------------
			targetObject.position=Vector3.Lerp(targetObject.position,targetCenterPos,Mathf.Pow(timeCount,2));
	    }
	}
	
	void MoveWithInertia()
	{
			if(rotateDirectionX>0)
			{
				if(lastMouseX>0)
					lastMouseX-=Time.deltaTime*2/aroundInertia;
				if(lastMouseX<0)
					lastMouseX=0;
			}
			if(rotateDirectionX<0)
			{
				if(lastMouseX<0)
					lastMouseX+=Time.deltaTime*2/aroundInertia;
				if(lastMouseX>0)
					lastMouseX=0;
			}
			if(rotateDirectionY>0)
			{
				if(lastMouseY>0)
					lastMouseY-=Time.deltaTime*2/aroundInertia;
				if(lastMouseY<0)
					lastMouseY=0;
			}
			if(rotateDirectionY<0)
			{
				if(lastMouseY<0)
					lastMouseY+=Time.deltaTime*2/aroundInertia;
				if(lastMouseY>0)
					lastMouseY=0;
			}
			currentMouse.x +=lastMouseX *aroundMouseSpeed.x *0.3f;
			currentMouse.y -=lastMouseY *aroundMouseSpeed.y *0.3f;
			currentMouse.y = ClampAngle (currentMouse.y, mouseYLimit.x, mouseYLimit.y);
			
			transform.rotation = Quaternion.Euler(currentMouse.y, currentMouse.x, 0);
			transform.position = Quaternion.Euler(currentMouse.y, currentMouse.x, 0) * new Vector3(0,0, -distance) + targetObject.position;
	}
	
	float ClampAngle (float angle,float min,float max)
	{
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	void ReceiveXY(Vector2 theXY)
	{
		lastMouseX=theXY.x;
		lastMouseY=theXY.y;
	}
	
	void Format()
	{
		Ray theRay = myCamera.ScreenPointToRay (new Vector3(Screen.width*0.5f,Screen.height*0.5f,0));
		RaycastHit[] hits = Physics.RaycastAll (theRay);
		for (int i = 0; i < hits.Length; i++) {
			if (hits [i].collider == dragArea) {
				targetObject.position = hits [i].point;
				targetCenterPos = hits [i].point;
				distance = aroundViewPoints [aroundCurrentID].y;
				targetDistance = aroundViewPoints [aroundCurrentID].y;
				targetRot=new Vector2(transform.eulerAngles.y,aroundViewPoints[aroundCurrentID].x);
				return;
			}
		}
	}
	public float GetCurrentDistance()
	{
		return distance;
	}
	public Vector3 GetCenterPos()
	{
		return targetCenterPos;
	}
	public Vector2 GetCurrentRot()
	{
		return currentMouse;
	}
}
