using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class CameraAroundWithInertia : MonoBehaviour {

	public Transform targetObject;
	public float autoSpeed=0;
	public float inertia=0.618f;
	public float currentDistance=10;
	public float advanceSpeed=10;
	public Vector2 mouseSpeed=new Vector2(16,16);
	public Vector2 mouseLimit=new Vector2(0,80);
	public Vector2 advanceLimit=new Vector2(5,25);
	
	private Vector2 currentMouse;
	private float lastMouseX,rotateDirectionX,lastMouseY,rotateDirectionY;
	private float targetDistance,distanceTimeCount;

	void Start () 
	{
		Vector3 angles = transform.localEulerAngles;
	    currentMouse.x = angles.y;
	    currentMouse.y = angles.x;
		
		if(currentDistance<advanceLimit.x||currentDistance>advanceLimit.y)   //chushi de distance bu zai fanwei nei
			currentDistance=(advanceLimit.x+advanceLimit.y)/2;
		targetDistance=currentDistance;
		
		if(inertia<=0)   //will output error
			inertia=0.01f;
	}
	
	void Update () 
	{
	    if (targetObject)  
		{
			if(Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject ())               //shou dong xuan zhuan
			{
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
			else if(autoSpeed!=0)         //auto
			{
				lastMouseX=0;
				lastMouseY=0;
				currentMouse.x += autoSpeed*Time.deltaTime; 
				transform.localRotation = Quaternion.Euler(currentMouse.y, currentMouse.x, 0);
				transform.position = Quaternion.Euler(currentMouse.y, currentMouse.x, 0) * new Vector3(0,0, -currentDistance) + targetObject.position;
			}
			else                     //guan xing
				MoveWithInertia();
			
			if(targetDistance>advanceLimit.x&&Input.GetAxis("Mouse ScrollWheel")>0 && !EventSystem.current.IsPointerOverGameObject ())    //Limit the Distance
			{
				targetDistance-=Input.GetAxis("Mouse ScrollWheel")*advanceSpeed;
				distanceTimeCount=0;
			}
			if(targetDistance<advanceLimit.y&&Input.GetAxis("Mouse ScrollWheel")<0 && !EventSystem.current.IsPointerOverGameObject ())
			{
				targetDistance-=Input.GetAxis("Mouse ScrollWheel")*advanceSpeed;
				distanceTimeCount=0;
			}
			if(targetDistance>advanceLimit.y)
				targetDistance=advanceLimit.y;
			if(targetDistance<advanceLimit.x)
				targetDistance=advanceLimit.x;
			
			currentDistance=Vector2.Lerp(new Vector2(currentDistance,0),new Vector2(targetDistance,0),distanceTimeCount).x;   //Lerp the distance
			if(distanceTimeCount<1)    //Lerp Speed
				distanceTimeCount+=Time.deltaTime/2;
	    }
	}
	
	void MoveWithInertia()
	{
		if(rotateDirectionX>0)
		{
			if(lastMouseX>0)
				lastMouseX-=Time.deltaTime*2/inertia;
			if(lastMouseX<0)
				lastMouseX=0;
		}
		if(rotateDirectionX<0)
		{
			if(lastMouseX<0)
				lastMouseX+=Time.deltaTime*2/inertia;
			if(lastMouseX>0)
				lastMouseX=0;
		}
		if(rotateDirectionY>0)
		{
			if(lastMouseY>0)
				lastMouseY-=Time.deltaTime*2/inertia;
			if(lastMouseY<0)
				lastMouseY=0;
		}
		if(rotateDirectionY<0)
		{
			if(lastMouseY<0)
				lastMouseY+=Time.deltaTime*2/inertia;
			if(lastMouseY>0)
				lastMouseY=0;
		}

		currentMouse.x +=lastMouseX *mouseSpeed.x *0.3f;
		currentMouse.y -=lastMouseY *mouseSpeed.y *0.3f;
		currentMouse.y = ClampAngle(currentMouse.y, mouseLimit.x, mouseLimit.y);
		
		transform.localRotation = Quaternion.Euler(currentMouse.y, currentMouse.x, 0);
		transform.position = Quaternion.Euler(currentMouse.y, currentMouse.x, 0) * new Vector3(0,0, -currentDistance) + targetObject.position;
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
		//must -180<?<180
		currentMouse.y=theXY.x;
		currentMouse.x=theXY.y;
	}
}
