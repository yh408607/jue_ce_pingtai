using UnityEngine;
using System.Collections;

[RequireComponent (typeof(Camera))]
public class CY_EdgeCamera : MonoBehaviour 
{
	public int size=3;
	public float edge=0.1f;
	public Color outterColor=new Color(0.133f,1,0,1);
	public LayerMask outterLayer=0;
	
	private GameObject childCamera;
	private RenderTexture outterLineTexture;
//---------------------------------------------------
	public Shader blurShader;
    private Material blurMaterial;
	
	public Shader compositeShader;
	private Material compositeMaterial;
	
	public Shader cutoffShader;
	private Material cutoffMaterial;
	
	private Material outterLineMat;
//---------------------------------------------------
	
	void Start()
	{
		CreateChildCamera();
//--------------------------------------------
		FormatMaterial();
//--------------------------------------------
		if(!outterLineTexture)
		{
			outterLineTexture =  new RenderTexture( (int)GetComponent<Camera>().pixelWidth,(int)GetComponent<Camera>().pixelHeight, 16 );
			outterLineTexture.hideFlags = HideFlags.DontSave;
		}
	}
	
	void FormatMaterial()
	{
		compositeMaterial =new Material(compositeShader);
		compositeMaterial.hideFlags = HideFlags.HideAndDontSave;
		blurMaterial = new Material(blurShader);
		blurMaterial.hideFlags = HideFlags.HideAndDontSave;
		cutoffMaterial = new Material( cutoffShader );
		cutoffMaterial.hideFlags = HideFlags.HideAndDontSave;
//		outterLineMat=new Material("Shader\"Leon/OutLine/Temp\"{SubShader{Pass{Color("+outterColor.r +","+outterColor.g +","+outterColor.b +","+outterColor.a +")}}}");
		outterLineMat=new Material(Shader.Find("Leon/OutterLineTemp"));
		outterLineMat.SetColor ("_Color", outterColor);
		outterLineMat.hideFlags = HideFlags.HideAndDontSave;
		outterLineMat.shader.hideFlags =  HideFlags.HideAndDontSave;
	}
	
	void CreateChildCamera()
	{
		childCamera=GameObject.CreatePrimitive(PrimitiveType.Cube);
		childCamera.name="OutLineCamera";
		Destroy(childCamera.GetComponent<Collider>());
		Destroy(childCamera.GetComponent<Renderer>());
		Destroy(childCamera.GetComponent<MeshFilter>());
		childCamera.AddComponent<Camera>();
		childCamera.SetActive(false);
		childCamera.transform.position=transform.position;
		childCamera.transform.rotation=transform.rotation;
		childCamera.transform.localScale=transform.lossyScale;
		childCamera.GetComponent<Camera>().CopyFrom(transform.GetComponent<Camera>());
		childCamera.GetComponent<Camera>().cullingMask=outterLayer;
		childCamera.GetComponent<Camera>().backgroundColor=new Color(0,0,0,0);
		childCamera.GetComponent<Camera>().clearFlags=CameraClearFlags.SolidColor;
		//-------------------------------
		childCamera.transform.parent=transform;
	}
	void OnPreRender() 
	{
		childCamera.GetComponent<Camera>().targetTexture = outterLineTexture;
		childCamera.GetComponent<Camera>().RenderWithShader(outterLineMat.shader,"");
	}
	
	public void FourTapCone (RenderTexture source, RenderTexture dest, int iteration)
	{
		float off = 0.5f+iteration*edge;
		Graphics.BlitMultiTap (source, dest, blurMaterial,
            new Vector2( off, off),
			new Vector2(-off, off),
            new Vector2( off,-off),
            new Vector2(-off,-off)
		);
	}
	
	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		RenderTexture buffer = RenderTexture.GetTemporary(outterLineTexture.width, outterLineTexture.height, 0);
		RenderTexture buffer2 = RenderTexture.GetTemporary(outterLineTexture.width, outterLineTexture.height, 0);
		
		Graphics.Blit(outterLineTexture, buffer);

		bool oddEven = true;
		for(int i = 0; i < size; i++)
		{
			if( oddEven )
				FourTapCone (buffer, buffer2, i);
			else
				FourTapCone (buffer2, buffer, i);
			oddEven = !oddEven;
		}
		Graphics.Blit(source,destination);
		if( oddEven ){
			Graphics.Blit(outterLineTexture,buffer, cutoffMaterial);
			Graphics.Blit(buffer,destination,compositeMaterial);
		}
		else{
			Graphics.Blit(outterLineTexture,buffer2, cutoffMaterial);
			Graphics.Blit(buffer2,destination,compositeMaterial);
		}		
		
		RenderTexture.ReleaseTemporary(buffer);
		RenderTexture.ReleaseTemporary(buffer2);
	}
}
