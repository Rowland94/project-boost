using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float thrustValue = 100f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip crash;
    [SerializeField] AudioClip levelLoad;
    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidbody;
    AudioSource audiosource;
    


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();
    }

    bool isTransitioning = false;
    bool collisionsDisabled  = false;

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {
            RespondtoRotateInput();
            RespondtoThrustInput();
        }
        if (Debug.isDebugBuild)
        {
        RespondToDebugKeys();
        }
        
        
    }

    void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // toggle collision on and off.
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsDisabled) { return; };
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                successSequence();
                break;
            default:
                failureSequence();
                break;
        }
        
    }

    void failureSequence()
    {
        isTransitioning = true;
        audiosource.Stop();
        audiosource.PlayOneShot(crash);
        deathParticles.Play();
        Invoke("LoadFirstLevel", 1f);
    }

    void successSequence()
    {
        isTransitioning = true;
        audiosource.Stop();
        audiosource.PlayOneShot(levelLoad);
        successParticles.Play();
        Invoke("LoadNextLevel", 1f);
    }

    void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    void LoadNextLevel()
    {
        int currentScenceIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentScenceIndex+1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        else
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        
    }

    void RespondtoThrustInput()
    {
        if (Input.GetKey(KeyCode.Space)) //can thrust while rotating
        {
            ApplyThrust();
        }
        else
        {
            StopThrust();
        }
    }

    void StopThrust()
    {
        mainEngineParticles.Stop();
        audiosource.Stop();
    }

    void ApplyThrust()
    {
        float thrustingThisFrame = thrustValue * Time.deltaTime;
        rigidbody.AddRelativeForce(Vector3.up * thrustingThisFrame);
        if (!audiosource.isPlaying)
        {
            audiosource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }

    void RespondtoRotateInput()
    {
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        rigidbody.angularVelocity = Vector3.zero;
      
        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
    }

   
}
