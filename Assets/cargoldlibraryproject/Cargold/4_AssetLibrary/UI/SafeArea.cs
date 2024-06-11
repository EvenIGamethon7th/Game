using Sirenix.OdinInspector;
using UnityEngine;
namespace Crystal
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel.
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        #region Simulations
        /// <summary>
        /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
        /// </summary>
        public enum SimDevice
        {
            /// <summary>
            /// Don't use a simulated safe area - GUI will be full screen as normal.
            /// </summary>
            None,
            /// <summary>
            /// Simulate the iPhone X and Xs (identical safe areas).
            /// </summary>
            iPhoneX,
            /// <summary>
            /// Simulate the iPhone Xs Max and XR (identical safe areas).
            /// </summary>
            iPhoneXsMax,
            /// <summary>
            /// Simulate the Google Pixel 3 XL using landscape left.
            /// </summary>
            Pixel3XL_LSL,
            /// <summary>
            /// Simulate the Google Pixel 3 XL using landscape right.
            /// </summary>
            Pixel3XL_LSR
        }
        /// <summary>
        /// Simulation mode for use in editor only. This can be edited at runtime to toggle between different safe areas.
        /// </summary>
        [SerializeField] public static SimDevice Sim = SimDevice.None;
        [Range( 0f, 1f )]
        public static float WidthRate = 1f;
        [Range( 0f, 1f )]
        public static float HeightRate = 1f;
        [Header("노치일때만 적용"), Range(0f, 2f)]
        public float widthSafeRate = 1f;
        [Range(0f, 2f)]
        public float heightSafeRate = 1f;
        [Header("공통 변수")]
        public bool isMinYIgnore;
        public bool isMaxYIgnore;
        /// <summary>
        /// Normalised safe areas for iPhone X with Home indicator (ratios are identical to iPhone Xs). Absolute values:
        ///  PortraitU x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436;
        ///  PortraitD x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125;
        ///  LandscapeR x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        static Rect[] NSA_iPhoneX = new Rect[]
        {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
        };
        /// <summary>
        /// Normalised safe areas for iPhone Xs Max with Home indicator (ratios are identical to iPhone XR). Absolute values:
        ///  PortraitU x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688;
        ///  PortraitD x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242;
        ///  LandscapeR x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        static Rect[] NSA_iPhoneXsMax = new Rect[]
        {
            new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),  // Portrait
            new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
        };
        /// <summary>
        /// Normalised safe areas for Pixel 3 XL using landscape left. Absolute values:
        ///  PortraitU x=0, y=0, w=1440, h=2789 on full extents w=1440, h=2960;
        ///  PortraitD x=0, y=0, w=1440, h=2789 on full extents w=1440, h=2960;
        ///  LandscapeL x=171, y=0, w=2789, h=1440 on full extents w=2960, h=1440;
        ///  LandscapeR x=0, y=0, w=2789, h=1440 on full extents w=2960, h=1440.
        ///  Aspect Ratio: 18.5:9.
        /// </summary>
        static Rect[] NSA_Pixel3XL_LSL = new Rect[]
        {
            new Rect (0f, 0f, 1f, 2789f / 2960f),  // Portrait
            new Rect (0f, 0f, 2789f / 2960f, 1f)  // Landscape
        };
        /// <summary>
        /// Normalised safe areas for Pixel 3 XL using landscape right. Absolute values and aspect ratio same as above.
        /// </summary>
        static Rect[] NSA_Pixel3XL_LSR = new Rect[]
        {
            new Rect (0f, 0f, 1f, 2789f / 2960f),  // Portrait
            new Rect (171f / 2960f, 0f, 2789f / 2960f, 1f)  // Landscape
        };
        #endregion

        [SerializeField] private RectTransform[] targetRtrfArr;
        private Rect lastSafeArea = new Rect(0, 0, 0, 0);
        private Vector3 lastScreenSize = new Vector3(0, 0);
        private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;
        [SerializeField] private bool conformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] private bool conformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)

        void Awake()
        {
            this.Refresh();
        }
#if UNITY_EDITOR
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (Sim == SimDevice.iPhoneX)
                    Sim = SimDevice.None;
                else if (Sim == SimDevice.None)
                    Sim = SimDevice.iPhoneX;
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                this.Refresh();
            }
        }
#endif
        [Button]
        void Refresh()
        {
            Rect _safeArea = this.GetSafeArea();
            if (_safeArea != this.lastSafeArea
                || Screen.width != this.lastScreenSize.x
                || Screen.height != this.lastScreenSize.y
                || Screen.orientation != this.lastOrientation)
            {
                // Fix for having auto-rotate off and manually forcing a screen orientation.
                // See https://forum.unity.com/threads/569236/#post-4473253 and https://forum.unity.com/threads/569236/page-2#post-5166467
                this.lastScreenSize.x = Screen.width;
                this.lastScreenSize.y = Screen.height;
                this.lastOrientation = Screen.orientation;



                this.lastSafeArea = _safeArea;
                // Ignore x-axis?
                if (this.conformX == false)
                {
                    _safeArea.x = 0;
                    _safeArea.width = Screen.width;
                }

                // Ignore y-axis?
                if (this.conformY == false)
                {
                    _safeArea.y = 0;
                    _safeArea.height = Screen.height;
                }

                // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
                Vector2 _anchorMin = _safeArea.position;
                Vector2 _anchorMax = _safeArea.position + _safeArea.size;
                _anchorMin.x /= Screen.width;
                _anchorMin.y /= Screen.height;
                _anchorMax.x /= Screen.width;
                _anchorMax.y /= Screen.height;

                if (this.isMinYIgnore == true)
                    _anchorMin.y = 0f;

                if (this.isMaxYIgnore == true)
                    _anchorMin.y = 1f;

                foreach (RectTransform _targetRtr in this.targetRtrfArr)
                {
                    if (_targetRtr != null)
                    {
                        _targetRtr.anchorMin = _anchorMin;
                        _targetRtr.anchorMax = _anchorMax;
                    }
                }
            }
        }
        public Rect GetSafeArea()
        {
            Rect _safeArea = Screen.safeArea;
            if (Application.isEditor && Sim != SimDevice.None)
            {
                Rect _nsaRect = new Rect(0, 0, Screen.width, Screen.height);
                switch (Sim)
                {
                    case SimDevice.iPhoneX:
                        _nsaRect = Screen.height <= Screen.width ? NSA_iPhoneX[1] : NSA_iPhoneX[0];
                        break;
                    case SimDevice.iPhoneXsMax:
                        _nsaRect = Screen.height <= Screen.width ? NSA_iPhoneXsMax[1] : NSA_iPhoneXsMax[0];
                        break;
                    case SimDevice.Pixel3XL_LSL:
                        _nsaRect = Screen.height <= Screen.width ? NSA_Pixel3XL_LSL[1] : NSA_Pixel3XL_LSL[0];
                        break;
                    case SimDevice.Pixel3XL_LSR:
                        _nsaRect = Screen.height <= Screen.width ? NSA_Pixel3XL_LSR[1] : NSA_Pixel3XL_LSR[0];
                        break;

                    default:
                        break;
                }

                _safeArea = new Rect(Screen.width * _nsaRect.x, Screen.height * _nsaRect.y, Screen.width * _nsaRect.width, Screen.height * _nsaRect.height);
            }
            float _wRate = WidthRate;
            if (this.widthSafeRate != 1f && Screen.width > _safeArea.width)
            {
                float _rate = (Screen.width - _safeArea.width) / Screen.width * 0.5f;
                if (this.widthSafeRate - 1f < _rate)
                    _wRate = WidthRate * this.widthSafeRate;
            }
            float _hRate = HeightRate;
            if (this.heightSafeRate != 1f && Screen.height > _safeArea.height)
            {
                float rate = (Screen.height - _safeArea.height) / Screen.height * 0.5f;
                if (this.heightSafeRate - 1f < rate)
                    _hRate = HeightRate * this.heightSafeRate;
            }

            _safeArea.x *= _wRate;
            _safeArea.y *= _hRate;
            _safeArea.width *= _wRate;
            _safeArea.height *= _hRate;
            return _safeArea;
        }
    }
}