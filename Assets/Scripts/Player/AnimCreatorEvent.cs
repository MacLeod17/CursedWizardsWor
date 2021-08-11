using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCreatorEvent : MonoBehaviour
{
	[System.Serializable]
	public struct Creator
	{
		public string id;
		public Transform locator;
	}

	public Creator[] creators;

	Dictionary<string, Transform> registry = new Dictionary<string, Transform>();

	public void Start()
	{
		foreach (Creator creator in creators)
		{
			registry.Add(creator.id, creator.locator);
		}
	}

	public void OnCreate(AnimationEvent animationEvent)
	{
		string id = animationEvent.stringParameter;
		if (registry.ContainsKey(id))
		{
			GameObject gameObject;
			bool parent = animationEvent.intParameter == 1;
			if (parent)
			{
				gameObject = (GameObject)Instantiate(animationEvent.objectReferenceParameter, registry[id]);
			}
			else
			{
				gameObject = (GameObject)Instantiate(animationEvent.objectReferenceParameter, registry[id].position, registry[id].rotation);
			}
			if (animationEvent.floatParameter > 0)
			{
				Object.Destroy(gameObject, animationEvent.floatParameter);
			}
		}
	}
}
