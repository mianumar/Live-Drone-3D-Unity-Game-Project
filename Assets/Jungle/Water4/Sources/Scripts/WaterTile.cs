using UnityEngine;

[ExecuteInEditMode]
public class WaterTile : MonoBehaviour 
{
	// public PlanarReflection reflection;
	public WaterBase waterBase;
	
	public void Start () 
	{
		AcquireComponents();
	}
	
	private void AcquireComponents() 
	{
		
		if (!waterBase) {
			if (transform.parent)
				waterBase = (WaterBase)transform.parent.GetComponent<WaterBase>();
			else
				waterBase = (WaterBase)transform.GetComponent<WaterBase>();	
		}
	}
	
#if UNITY_EDITOR
	public void Update () 
	{
		AcquireComponents();
	}
#endif
	
	public void OnWillRenderObject() 
	{
		if (waterBase)
			waterBase.WaterTileBeingRendered(transform, Camera.current);		
	}
}
