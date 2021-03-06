﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ObjectState
{

    //Transform
    public Vector3 position;//2017-10-10: actually stores the localPosition
    public Vector3 localScale;
    public Quaternion rotation;//2017-10-10: actually stores the localRotation
    //RigidBody2D
    public Vector2 velocity;
    public float angularVelocity;
    //Saveable Object
    public List<SavableObject> soList;
    //Name
    public string objectName;
    public string sceneName;

    private Rigidbody2D rb2d;
    private GameObject go;
    private List<SavableMonoBehaviour> smbList;

    public ObjectState() { }
    public ObjectState(GameObject goIn)
    {
        go = goIn;
        objectName = go.name;
        sceneName = go.scene.name;
        rb2d = go.GetComponent<Rigidbody2D>();
        smbList = new List<SavableMonoBehaviour>();
        smbList.AddRange(go.GetComponents<SavableMonoBehaviour>());
    }

    public void saveState()
    {
        position = go.transform.localPosition;
        localScale = go.transform.localScale;
        rotation = go.transform.localRotation;
        if (rb2d != null)
        {
            velocity = rb2d.velocity;
            angularVelocity = rb2d.angularVelocity;
        }
        soList = new List<SavableObject>();
        foreach (SavableMonoBehaviour smb in smbList)
        {
            this.soList.Add(smb.getSavableObject());
        }
    }
    public void loadState()
    {
        getGameObject();//finds and sets the game object
        if (go == null)
        {
            return;//don't load the state if go is null
        }
        go.transform.localPosition = position;
        go.transform.localScale = localScale;
        go.transform.localRotation = rotation;
        rb2d = go.GetComponent<Rigidbody2D>();
        if (rb2d != null)
        {
            rb2d.velocity = velocity;
            rb2d.angularVelocity = angularVelocity;
        }
        foreach (SavableObject so in this.soList)
        {
            SavableMonoBehaviour smb = (SavableMonoBehaviour)go.GetComponent(so.getSavableMonobehaviourType());
            if (smb == null)
            {
                if (so.isSpawnedScript)
                {
                    smb = (SavableMonoBehaviour)so.addScript(go);
                }
                else
                {
                    throw new UnityException("Object " + go + " is missing non-spawnable script " + so.scriptType);
                }
            }
            smb.acceptSavableObject(so);
        }
    }

    //
    //Retrieves the variable, go,
    //and if go is null, it finds the correct GameObject and sets go
    //
    public GameObject getGameObject()
    {
        if (go == null || ReferenceEquals(go, null))//2016-11-20: reference equals test copied from an answer by sindrijo: http://answers.unity3d.com/questions/13840/how-to-detect-if-a-gameobject-has-been-destroyed.html
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                foreach (GameObject sceneGo in scene.GetRootGameObjects())
                {
                    if (sceneGo.name.Equals(objectName))
                    {
                        this.go = sceneGo;
                        break;
                    }
                    else
                    {
                        foreach (Transform childTransform in sceneGo.GetComponentsInChildren<Transform>())
                        {
                            GameObject childGo = childTransform.gameObject;
                            if (childGo.name.Equals(objectName))
                            {
                                this.go = childGo;
                                break;
                            }
                        }
                    }
                }
                if (go == null)
                {
                    foreach (GameObject dgo in GameManager.getForgottenObjects())
                    {
                        if (dgo != null && dgo.name == objectName && dgo.scene.name == sceneName)
                        {
                            go = dgo;
                            break;
                        }
                    }
                }
                //if not found, do stuff
                if (go == null || ReferenceEquals(go, null))
                {
                    //if is spawned object, make it
                    foreach (SavableObject so in soList)
                    {
                        if (so.isSpawnedObject)
                        {
                            GameObject spawned = so.spawnObject();
                            if (spawned.scene.name != sceneName)
                            {
                                SceneManager.MoveGameObjectToScene(spawned, scene);
                            }
                            foreach (Transform t in spawned.transform)
                            {
                                if (t.gameObject.name == this.objectName)
                                {
                                    go = t.gameObject;
                                    break;
                                }
                            }
                            if (go == null)
                            {
                                go = spawned;
                                go.name = this.objectName;
                            }
                            GameManager.addObject(spawned);
                            break;
                        }
                    }
                }
            }
            else
            {
                go = null;
            }
        }
        return go;
    }
}
