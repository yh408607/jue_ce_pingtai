using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CY_EdgeEffect : MonoBehaviour {

	public int targetLayer;
	public float hideTime=0.1f;
	
	private int myLayer;
	private float timeRecord;
	private bool isChanged=false;
	private Transform[] allMyChildren;
	
	void Start()
	{
		myLayer=gameObject.layer;
		allMyChildren=GetComponentsInChildren<Transform>();
		foreach(Transform temp in allMyChildren)
			if(temp!=transform)
				if(temp.GetComponent<Collider>())
					temp.GetComponent<Collider>().enabled=false;
	}
	
	void OnMouseOver()
	{
		if(EventSystem.current.IsPointerOverGameObject())
			return;
		timeRecord=Time.timeSinceLevelLoad;
		foreach(Transform temp in allMyChildren)
			temp.gameObject.layer=targetLayer;
		isChanged=true;
	}
	void Update()
	{
		if(isChanged&&Time.timeSinceLevelLoad-timeRecord>hideTime)
		{
			foreach(Transform temp in allMyChildren)
				temp.gameObject.layer=myLayer;
			isChanged=false;
		}
	}
}
