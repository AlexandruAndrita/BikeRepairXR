using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

/// <summary>
/// Manages instruction navigation for the ExperimentInstructions canvas.
/// Allows users to navigate through multiple instruction pages using Next/Prev buttons.
/// Each instruction can have an associated video clip.
/// </summary>
public class ExperimentInstructionController : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Text component to display instructions")]
    public TextMeshProUGUI instructionText;
    
    [Tooltip("Next button to go to the next instruction")]
    public Button nextButton;
    
    [Tooltip("Previous/Back button to go to the previous instruction")]
    public Button prevButton;
    
    [Header("Video References")]
    [Tooltip("VideoPlayer component for playing instruction videos")]
    public VideoPlayer videoPlayer;
    
    [Tooltip("RawImage component for displaying video output")]
    public RawImage videoImage;
    
    [Header("Instructions")]
    [Tooltip("Array of instruction strings for each page")]
    [TextArea(3, 10)]
    public string[] instructions = new string[]
    {
        "Welcome! Follow the instructions to complete the task.",
        "Step 1: Locate the pedal that needs to be replaced.",
        "Step 2: Use the wrench to loosen the pedal bolt.",
        "Step 3: Remove the old pedal carefully.",
        "Step 4: Install the new pedal and tighten securely.",
        "You're all set! Great job completing the task."
    };
    
    [Header("Videos")]
    [Tooltip("Array of video clips corresponding to each instruction. Leave null for instructions without video.")]
    public VideoClip[] instructionVideos;
    
    [Header("Settings")]
    [Tooltip("Whether to loop back to the first instruction after the last one")]
    public bool loopInstructions = false;
    
    [Tooltip("Whether to loop the current video while on its instruction")]
    public bool loopCurrentVideo = true;
    
    [Header("Controller Button Mapping")]
    [Tooltip("Use left controller X button for Prev and Y button for Next")]
    public bool enableControllerButtons = true;

    [Header("Ray-Based Controller Interaction")]
    [Tooltip("Allow index trigger rays to click UI buttons on this instruction canvas")]
    public bool enableRayTriggerInteraction = true;

    [Tooltip("Maximum ray distance in meters for trigger interactions")]
    public float maxRayDistance = 10f;

    private int currentIndex = 0;
    private Canvas targetCanvas;
    private Transform leftController;
    private Transform rightController;

    void Update()
    {
        if (enableControllerButtons)
        {
            // Y button (left controller) -> Next
            if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
                OnNextClicked();

            // X button (left controller) -> Prev
            if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.LTouch))
                OnPrevClicked();
        }

        if (enableRayTriggerInteraction)
        {
            bool leftTrigger = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            bool rightTrigger = OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch);

            if (leftTrigger) TryClickButton(leftController);
            if (rightTrigger) TryClickButton(rightController);
        }
    }

    void Start()
    {
        targetCanvas = GetComponent<Canvas>();

        OVRCameraRig rig = FindFirstObjectByType<OVRCameraRig>();
        if (rig != null)
        {
            leftController = rig.leftControllerAnchor;
            rightController = rig.rightControllerAnchor;
        }

        // Auto-find UI elements if not assigned
        if (instructionText == null)
        {
            GameObject textObj = GameObject.Find("InstructionText");
            if (textObj != null) instructionText = textObj.GetComponent<TextMeshProUGUI>();
        }
        
        if (nextButton == null)
        {
            GameObject nextBtnObj = GameObject.Find("NextButton");
            if (nextBtnObj != null) nextButton = nextBtnObj.GetComponent<Button>();
        }
        
        if (prevButton == null)
        {
            GameObject prevBtnObj = GameObject.Find("PrevButton");
            if (prevBtnObj != null) prevButton = prevBtnObj.GetComponent<Button>();
        }
        
        // Auto-find video components if not assigned
        if (videoPlayer == null)
        {
            videoPlayer = GetComponentInChildren<VideoPlayer>(true);
            if (videoPlayer == null)
            {
                GameObject videoObj = GameObject.Find("VideoImage");
                if (videoObj != null) videoPlayer = videoObj.GetComponent<VideoPlayer>();
            }
        }
        
        if (videoImage == null)
        {
            videoImage = GetComponentInChildren<RawImage>(true);
            if (videoImage == null)
            {
                GameObject videoObj = GameObject.Find("VideoImage");
                if (videoObj != null) videoImage = videoObj.GetComponent<RawImage>();
            }
        }
        
        // Initialize video player settings
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = loopCurrentVideo;
            videoPlayer.playOnAwake = false;
            
            // Set up render texture if using RawImage
            if (videoImage != null && videoPlayer.targetTexture == null)
            {
                videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            }
        }
        
        // Add listeners to buttons
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNextClicked);
        }
        else
        {
            Debug.LogWarning("ExperimentInstructionController: NextButton not found!");
        }
        
        if (prevButton != null)
        {
            prevButton.onClick.RemoveAllListeners();
            prevButton.onClick.AddListener(OnPrevClicked);
        }
        else
        {
            Debug.LogWarning("ExperimentInstructionController: PrevButton not found!");
        }
        
        // Display first instruction
        UpdateInstructionDisplay();
        
        Debug.Log($"ExperimentInstructionController initialized with {instructions.Length} instructions");
    }

    void TryClickButton(Transform anchor)
    {
        if (anchor == null || targetCanvas == null) return;

        Ray ray = new Ray(anchor.position, anchor.forward);
        Plane canvasPlane = new Plane(targetCanvas.transform.forward, targetCanvas.transform.position);

        if (!canvasPlane.Raycast(ray, out float distance) || distance > maxRayDistance) return;

        Vector3 hitPoint = ray.GetPoint(distance);

        foreach (Button button in targetCanvas.GetComponentsInChildren<Button>(true))
        {
            if (!button.IsInteractable()) continue;

            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != null && WorldPointInRect(rect, hitPoint))
            {
                button.onClick.Invoke();
                return;
            }
        }
    }

    static bool WorldPointInRect(RectTransform rectTransform, Vector3 worldPoint)
    {
        Vector3 localPoint = rectTransform.InverseTransformPoint(worldPoint);
        return rectTransform.rect.Contains(new Vector2(localPoint.x, localPoint.y));
    }
    
    void OnNextClicked()
    {
        if (instructions.Length == 0) return;
        
        currentIndex++;
        
        if (currentIndex >= instructions.Length)
        {
            if (loopInstructions)
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex = instructions.Length - 1;
                Debug.Log("Reached last instruction");
            }
        }
        
        UpdateInstructionDisplay();
        Debug.Log($"Next clicked. Now showing instruction {currentIndex + 1}/{instructions.Length}");
    }
    
    void OnPrevClicked()
    {
        if (instructions.Length == 0) return;
        
        currentIndex--;
        
        if (currentIndex < 0)
        {
            if (loopInstructions)
            {
                currentIndex = instructions.Length - 1;
            }
            else
            {
                currentIndex = 0;
                Debug.Log("Already at first instruction");
            }
        }
        
        UpdateInstructionDisplay();
        Debug.Log($"Previous clicked. Now showing instruction {currentIndex + 1}/{instructions.Length}");
    }
    
    void UpdateInstructionDisplay()
    {
        if (instructionText != null && instructions.Length > 0 && currentIndex >= 0 && currentIndex < instructions.Length)
        {
            instructionText.text = instructions[currentIndex];
        }
        
        // Update video for current instruction
        UpdateVideoDisplay();
        
        // Update button interactability
        UpdateButtonStates();
    }
    
    void UpdateVideoDisplay()
    {
        if (videoPlayer == null) return;
        
        // Stop current video
        videoPlayer.Stop();
        
        // Check if there's a video for this instruction
        if (instructionVideos != null && currentIndex >= 0 && currentIndex < instructionVideos.Length)
        {
            VideoClip clip = instructionVideos[currentIndex];
            if (clip != null)
            {
                videoPlayer.clip = clip;
                videoPlayer.isLooping = loopCurrentVideo;
                videoPlayer.Play();
                
                // Show video image
                if (videoImage != null)
                    videoImage.enabled = true;
            }
            else
            {
                // No video for this instruction
                videoPlayer.clip = null;
                if (videoImage != null)
                    videoImage.enabled = false;
            }
        }
        else
        {
            // No video array or index out of range
            videoPlayer.clip = null;
            if (videoImage != null)
                videoImage.enabled = false;
        }
    }
    
    void UpdateButtonStates()
    {
        if (!loopInstructions)
        {
            // Disable prev button on first page
            if (prevButton != null)
            {
                prevButton.interactable = currentIndex > 0;
            }
            
            // Disable next button on last page
            if (nextButton != null)
            {
                nextButton.interactable = currentIndex < instructions.Length - 1;
            }
        }
    }
    
    /// <summary>
    /// Set the current instruction index programmatically
    /// </summary>
    public void SetInstructionIndex(int index)
    {
        if (index >= 0 && index < instructions.Length)
        {
            currentIndex = index;
            UpdateInstructionDisplay();
        }
        else
        {
            Debug.LogWarning($"Invalid instruction index: {index}. Valid range: 0 to {instructions.Length - 1}");
        }
    }
    
    /// <summary>
    /// Get the current instruction index
    /// </summary>
    public int GetCurrentIndex()
    {
        return currentIndex;
    }
    
    /// <summary>
    /// Get the total number of instructions
    /// </summary>
    public int GetInstructionCount()
    {
        return instructions.Length;
    }
    
    void OnDestroy()
    {
        // Clean up listeners
        if (nextButton != null)
            nextButton.onClick.RemoveListener(OnNextClicked);
        if (prevButton != null)
            prevButton.onClick.RemoveListener(OnPrevClicked);
        
        // Clean up video player
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
        }
    }
}
