using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


#pragma warning disable 0618 // Disabled warning due to SetVertices being deprecated until new release with SetMesh() is available.

namespace TMPro.Examples
{

    public class TMP_DragWordMark : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IPointerClickHandler//, IPointerUpHandler
    {
        public PopDragWordSearchView dragWordSearchView;
        public RectTransform contentTrans;


        // Color32 highLightColor = new Color32(200, 200, 30, 255);// Color.yellow;
        private TextMeshProUGUI m_TextMeshPro;
        private Canvas m_Canvas;
        private Camera m_Camera;

        bool isHoveringObject = false;
        bool is_down = false;
        int charIndex = -1;
        int startCharIndex = -1;
        void Awake()
        {
            m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();


            m_Canvas = gameObject.GetComponentInParent<Canvas>();

            // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                m_Camera = null;
            else
                m_Camera = m_Canvas.worldCamera;

            // Create pop-up text object which is used to show the link information.
            //m_TextPopup_RectTransform = Instantiate(TextPopup_Prefab_01) as RectTransform;
            //m_TextPopup_RectTransform.SetParent(m_Canvas.transform, false);
            //m_TextPopup_TMPComponent = m_TextPopup_RectTransform.GetComponentInChildren<TextMeshProUGUI>();
            //m_TextPopup_RectTransform.gameObject.SetActive(false);
        }

        void LateUpdate()
        {
            if (isHoveringObject)
            {
                string originalText = m_TextMeshPro.text;
                if (Input.GetMouseButtonDown(0))
                {
                    is_down = true;
                    charIndex = TMP_TextUtilities.FindIntersectingCharacter(m_TextMeshPro, Input.mousePosition, m_Camera, false);

                    if (charIndex != -1)
                    {
                        startCharIndex = charIndex;
                    }
                }

                if (Input.GetMouseButton(0) && is_down)
                {
                    charIndex = TMP_TextUtilities.FindIntersectingCharacter(m_TextMeshPro, Input.mousePosition, m_Camera, false);

                    if (charIndex != -1 && (charIndex > startCharIndex))
                    {
                        //char ch = m_TextMeshPro.textInfo.characterInfo[charIndex].character;                      
                        int subTextLength = charIndex - startCharIndex + 1;
                        string subText = originalText.Substring(startCharIndex, subTextLength);
                        string highlight_tag = "<mark=#ffff00aa>" + subText + "</mark>";
                        string editText = originalText;
                        editText = editText.Replace(subText, highlight_tag);
                        m_TextMeshPro.text = editText;
                    }

                    if (charIndex != -1 && (charIndex < startCharIndex))
                    {
                        int subTextLength = startCharIndex - charIndex + 1;
                        string subText = originalText.Substring(charIndex, subTextLength);
                        string highlight_tag = "<mark=#ffff00aa>" + subText + "</mark>";
                        string editText = originalText;
                        editText = editText.Replace(subText, highlight_tag);
                        m_TextMeshPro.text = editText;
                    }
                }

                if (Input.GetMouseButtonUp(0))
                {
                    is_down = false;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isHoveringObject = true;
           // startWordIndex = TMP_TextUtilities.FindIntersectingWord(m_TextMeshPro, Input.mousePosition, m_Camera);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Reset();
        }
        protected void Reset()
        {
            isHoveringObject = false;
          //  hoverTime = 0;
          //  startWordIndex = -1;
        }
    }
}
