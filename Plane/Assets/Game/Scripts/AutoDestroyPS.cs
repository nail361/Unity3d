using UnityEngine;
using System.Collections;

public class AutoDestroyPS : MonoBehaviour
{
	private ParticleSystem ps;
	
	
	public void Awake()
	{
		ps = GetComponent<ParticleSystem>();
	}
	
	public void Update()
	{
		if(ps)
		{
			if(!ps.IsAlive())
			{
				Destroy(gameObject);
			}
		}
	}
}