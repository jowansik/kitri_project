using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MirzaBeig.ParticleSystems;

public class ParticleScaler : MonoBehaviour
{
    private ParticleSystem pS;
    private ParticleSystems pSs;

    public float scaleFactor;

    // Use this for initialization
    void Start ()
    {
        pS = GetComponent<ParticleSystem>();
        pSs = GetComponent<ParticleSystems>();        
	}
	
	// Update is called once per frame
	void Update ()
    {
       // pS.main.scalingMode = ParticleSystemScalingMode.Hierarchy;
	}
}
