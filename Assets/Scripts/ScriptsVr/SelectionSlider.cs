using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VRStandardAssets.Utils
{
    // This class works similarly to the SelectionRadial class except
    // it has a physical manifestation in the scene.  This can be
    // either a UI slider or a mesh with the SlidingUV shader.  The
    // functions as a bar that fills up whilst the user looks at it
    // and holds down the Fire1 button.
    public class SelectionSlider : MonoBehaviour
    {
        public event Action OnBarFilled;                                    // This event is triggered when the bar finishes filling.


        [SerializeField] private float m_Duration = 2f;                     // The length of time it takes for the bar to fill.
        [SerializeField] private Renderer m_Renderer;                       // Optional reference to a renderer (unnecessary if using a UI slider).
        [SerializeField] private RadialReticle m_SelectionRadial;
        private bool m_BarFilled;                                           // Whether the bar is currently filled.
        public bool m_GazeOver;                                            // Whether the user is currently looking at the bar.
        private float m_Timer;                                              // Used to determine how much of the bar should be filled.
        public Coroutine m_FillBarRoutine;                                 // Reference to the coroutine that controls the bar filling up, used to stop it if required.


        private const string k_SliderMaterialPropertyName = "_SliderValue"; // The name of the property on the SlidingUV shader that needs to be changed in order for it to fill.


        

        public IEnumerator WaitForBarToFill ()
        {
            
            // Currently the bar is unfilled.
            m_BarFilled = false;

            // Reset the timer and set the slider value as such.
            m_Timer = 0f;
            SetSliderValue (0f);

            // Keep coming back each frame until the bar is filled.
            while (!m_BarFilled)
            {
                yield return null;
            }
        }


        public IEnumerator FillBar ()
        {
            // When the bar starts to fill, reset the timer.
            m_Timer = 0f;

            // The amount of time it takes to fill is either the duration set in the inspector, or the duration of the radial.
            float fillTime = m_SelectionRadial != null ? m_SelectionRadial._radialDuration : m_Duration;

            // Until the timer is greater than the fill time...
            while (m_Timer < fillTime)
            {
                // ... add to the timer the difference between frames.
                m_Timer += Time.deltaTime;

                // Set the value of the slider or the UV based on the normalised time.
                SetSliderValue(m_Timer / fillTime);
                
                // Wait until next frame.
                yield return null;

                // If the user is still looking at the bar, go on to the next iteration of the loop.
                if (m_GazeOver)
                    continue;

                // If the user is no longer looking at the bar, reset the timer and bar and leave the function.
                m_Timer = 0f;
                SetSliderValue (0f);
                yield break;
            }

            // If the loop has finished the bar is now full.
            m_BarFilled = true;

            // If anything has subscribed to OnBarFilled call it now.
            if (OnBarFilled != null)
                OnBarFilled ();

           
        }


        public void SetSliderValue (float sliderValue)
        {
            
            // If there is a renderer set the shader's property to the given slider value.
            if(m_Renderer)
                m_Renderer.sharedMaterial.SetFloat (k_SliderMaterialPropertyName, sliderValue);
        }

}
}