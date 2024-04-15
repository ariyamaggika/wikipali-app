using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore;
using UnityEngine.TextCore.Text;


#pragma warning disable 0618 // Disabled warning due to SetVertices being deprecated until new release with SetMesh() is available.

namespace TMPro.Examples
{

    public class TMP_DragWordSearch : MonoBehaviour, IPointerDownHandler, IPointerUpHandler//, IPointerClickHandler//, IPointerUpHandler
    {
        public PopDragWordSearchView dragWordSearchView;
        public RectTransform contentTrans;
        public RectTransform scrollTrans;
        public RectTransform titleTrans;
        public RectTransform title2Trans;

        //public RectTransform TextPopup_Prefab_01;

        //private RectTransform m_TextPopup_RectTransform;
        private TextMeshProUGUI m_TextPopup_TMPComponent;
        private const string k_LinkText = "You have selected link <#ffff00>";
        private const string k_WordText = "Word Index: <#ffff00>";

        // Color32 highLightColor = new Color32(200, 200, 30, 255);// Color.yellow;
        private TextMeshProUGUI m_TextMeshPro;
        private Canvas m_Canvas;
        private Camera m_Camera;

        // Flags
        private bool isHoveringObject;
        private int m_selectedWord = -1;
        private int m_selectedLink = -1;
        private int m_lastIndex = -1;

        private Matrix4x4 m_matrix;

        private TMP_MeshInfo[] m_cachedMeshInfoVertexData;

        void Awake()
        {
            m_TextMeshPro = gameObject.GetComponent<TextMeshProUGUI>();


            m_Canvas = gameObject.GetComponentInParent<Canvas>();

            // Get a reference to the camera if Canvas Render Mode is not ScreenSpace Overlay.
            if (m_Canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                m_Camera = null;
            else
                m_Camera = m_Canvas.worldCamera;
            dragWordSearchView.Init();
            // Create pop-up text object which is used to show the link information.
            //m_TextPopup_RectTransform = Instantiate(TextPopup_Prefab_01) as RectTransform;
            //m_TextPopup_RectTransform.SetParent(m_Canvas.transform, false);
            //m_TextPopup_TMPComponent = m_TextPopup_RectTransform.GetComponentInChildren<TextMeshProUGUI>();
            //m_TextPopup_RectTransform.gameObject.SetActive(false);
        }


        void OnEnable()
        {
            // Subscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }

        void OnDisable()
        {
            // UnSubscribe to event fired when text object has been regenerated.
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }


        void ON_TEXT_CHANGED(Object obj)
        {
            if (obj == m_TextMeshPro)
            {
                // Update cached vertex data.
                m_cachedMeshInfoVertexData = m_TextMeshPro.textInfo.CopyMeshInfoVertexData();
            }
        }
        public void ClearPrevSearch()
        {
            //if (/*m_TextPopup_RectTransform != null &&*/ m_selectedWord != -1 && (wordIndex == -1 || wordIndex != m_selectedWord))
            {
                TMP_WordInfo wInfo = m_TextMeshPro.textInfo.wordInfo[m_selectedWord];

                // Iterate through each of the characters of the word.
                for (int i = 0; i < wInfo.characterCount; i++)
                {
                    int characterIndex = wInfo.firstCharacterIndex + i;

                    // Get the index of the material / sub text object used by this character.
                    int meshIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;

                    // Get the index of the first vertex of this character.
                    int vertexIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].vertexIndex;

                    // Get a reference to the vertex color
                    Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;

                    Color32 c = CommonTool.TintDownArticleColorRGB(vertexColors[vertexIndex + 0], 0.75f); //(1.33333f);

                    vertexColors[vertexIndex + 0] = c;
                    vertexColors[vertexIndex + 1] = c;
                    vertexColors[vertexIndex + 2] = c;
                    vertexColors[vertexIndex + 3] = c;
                }

                // Update Geometry
                m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

                m_selectedWord = -1;
            }


        }

        void LateUpdate()
        {
            if (isHoveringObject)
            {
                hoverTime += Time.deltaTime;
            }
            if (isHoveringObject && hoverTime > maxHoverTime)
            {
                Reset();
                // Check if Mouse Intersects any of the characters. If so, assign a random color.
                //#region Handle Character Selection

                //#endregion


                #region Word Selection Handling
                //Check if Mouse intersects any words and if so assign a random color to that word.
                int wordIndex = TMP_TextUtilities.FindIntersectingWord(m_TextMeshPro, Input.mousePosition, m_Camera);
                if (wordIndex != startWordIndex)
                    Reset();
                // Clear previous word selection.

                // Word Selection Handling
                if (wordIndex != -1 && wordIndex != m_selectedWord)//&& !(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
                {
                    m_selectedWord = wordIndex;

                    TMP_WordInfo wInfo = m_TextMeshPro.textInfo.wordInfo[wordIndex];
                    //todo 设置位置为单词最后一个字的位置
                    var lastCharInfo = m_TextMeshPro.textInfo.characterInfo[wInfo.lastCharacterIndex];

                    float screenSizeX = m_Canvas.GetComponent<RectTransform>().sizeDelta.x;
                    float btnXPos = lastCharInfo.bottomRight.x + dragWordSearchView.btnSizeX * 0.5f;// + screenSizeX * 0.5f;
                    if (btnXPos + dragWordSearchView.btnSizeX > screenSizeX * 0.5f)
                    {
                        btnXPos = screenSizeX * 0.5f - dragWordSearchView.btnSizeX * 0.5f;
                    }
                    RectTransform tmp_Trans = m_TextMeshPro.GetComponent<RectTransform>();
                    float textHeight = tmp_Trans.sizeDelta.y;

                    float tmpYOffset =/* -m_TextMeshPro.fontSize * 0.5f*/ -dragWordSearchView.btnSizeY * 0.5f
                        - titleTrans.GetComponent<RectTransform>().sizeDelta.y - title2Trans.GetComponent<RectTransform>().sizeDelta.y;//半个字体大小
                    float dragViewSize = m_Canvas.GetComponent<RectTransform>().sizeDelta.y;// (float)Screen.height;// dragWordSearchView.GetComponent<RectTransform>().sizeDelta.y;
                    //float y = lastCharInfo.bottomRight.y + contentTrans.localPosition.y + tmp_Trans.localPosition.y * 0.5f + tmpYOffset;
                    //Debug.LogError(tmp_Trans.localPosition.y);
                    //Debug.LogError(tmp_Trans.sizeDelta.y * 0.5f);
                    //Debug.LogError(tmp_Trans.localPosition.y - tmp_Trans.sizeDelta.y * 0.5f);
                    float y = (tmp_Trans.localPosition.y + tmp_Trans.sizeDelta.y * 0.5f) + dragViewSize * 0.5f - (textHeight - ((textHeight * 0.5f) + lastCharInfo.bottomRight.y) - contentTrans.localPosition.y) + tmpYOffset;// - dragViewSize*0.5f;
                    //float y = lastCharInfo.bottomRight.y  + tmpYOffset;

                    dragWordSearchView.DragWord(wInfo.GetWord(), this, new Vector3(btnXPos, y, 0));

                    //string text = m_TextMeshPro.text;
                    //int subTextLength = wInfo.characterCount;// + 1;
                    //string subTextFront = text.Substring(0, wInfo.firstCharacterIndex);// wInfo, subTextLength);
                    //string subTextLast = text.Substring(wInfo.lastCharacterIndex+1);// wInfo, subTextLength);
                    //string highlight_tag = "<mark=#ffff00aa>" + wInfo.GetWord() + "</mark>";

                    //m_TextMeshPro.text = subTextFront + highlight_tag + subTextLast;
                    //wInfo.textComponent.fontStyle |= FontStyles.Highlight;
                    //wInfo.textComponent.
                    //var startCharInfo = m_TextMeshPro.textInfo.characterInfo[wInfo.firstCharacterIndex];
                    //int vertexIndex2 = m_TextMeshPro.textInfo.characterInfo[wInfo.firstCharacterIndex].vertexIndex;
                    //DrawTextHighlight(startCharInfo.topLeft, lastCharInfo.bottomRight, wInfo.textComponent.textInfo,new Color32(0,0,0,255),ref vertexIndex2, new Color32(255, 255, 0, 200));
                    // Iterate through each of the characters of the word.
                    for (int i = 0; i < wInfo.characterCount; i++)
                    {
                        int characterIndex = wInfo.firstCharacterIndex + i;

                        // Get the index of the material / sub text object used by this character.
                        int meshIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].materialReferenceIndex;

                        int vertexIndex = m_TextMeshPro.textInfo.characterInfo[characterIndex].vertexIndex;
                        //  m_TextMeshPro.textInfo.characterInfo[characterIndex].highlightColor = new Color32(255, 255, 0, 100);
                        //  TMP_Offset highlightPadding = TMP_Offset.zero;
                        //  m_TextMeshPro.textInfo.characterInfo[characterIndex].highlightState = new HighlightState(new Color32(255, 255, 0,100), highlightPadding);
                        //// m_TextMeshPro.textInfo.characterInfo[characterIndex].style |= FontStyles.Highlight;
                        //  m_TextMeshPro.textInfo.characterInfo[characterIndex].style = FontStyles.Highlight;

                        // Get a reference to the vertex color
                        Color32[] vertexColors = m_TextMeshPro.textInfo.meshInfo[meshIndex].colors32;

                        // Color32 c = vertexColors[vertexIndex + 0].Tint(1.33333f);//(0.75f);
                        Color32 c = CommonTool.TintUpArticleColorRGB(vertexColors[vertexIndex + 0], 1.33333f); //(1.33333f);

                        vertexColors[vertexIndex + 0] = c;
                        vertexColors[vertexIndex + 1] = c;
                        vertexColors[vertexIndex + 2] = c;
                        vertexColors[vertexIndex + 3] = c;
                    }

                    // Update Geometry
                    m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);

                }
                #endregion

                //#region Example of Link Handling
                //// Check if mouse intersects with any links.
                //int linkIndex = TMP_TextUtilities.FindIntersectingLink(m_TextMeshPro, Input.mousePosition, m_Camera);

                //// Clear previous link selection if one existed.
                //if ((linkIndex == -1 && m_selectedLink != -1) || linkIndex != m_selectedLink)
                //{
                //    m_TextPopup_RectTransform.gameObject.SetActive(false);
                //    m_selectedLink = -1;
                //}

                //// Handle new Link selection.
                //if (linkIndex != -1 && linkIndex != m_selectedLink)
                //{
                //    m_selectedLink = linkIndex;

                //    TMP_LinkInfo linkInfo = m_TextMeshPro.textInfo.linkInfo[linkIndex];

                //    // Debug.Log("Link ID: \"" + linkInfo.GetLinkID() + "\"   Link Text: \"" + linkInfo.GetLinkText() + "\""); // Example of how to retrieve the Link ID and Link Text.

                //    Vector3 worldPointInRectangle;
                //    RectTransformUtility.ScreenPointToWorldPointInRectangle(m_TextMeshPro.rectTransform, Input.mousePosition, m_Camera, out worldPointInRectangle);

                //    switch (linkInfo.GetLinkID())
                //    {
                //        case "id_01": // 100041637: // id_01
                //            m_TextPopup_RectTransform.position = worldPointInRectangle;
                //            m_TextPopup_RectTransform.gameObject.SetActive(true);
                //            m_TextPopup_TMPComponent.text = k_LinkText + " ID 01";
                //            break;
                //        case "id_02": // 100041638: // id_02
                //            m_TextPopup_RectTransform.position = worldPointInRectangle;
                //            m_TextPopup_RectTransform.gameObject.SetActive(true);
                //            m_TextPopup_TMPComponent.text = k_LinkText + " ID 02";
                //            break;
                //    }
                //}
                //#endregion

            }
            else
            {
                // Restore any character that may have been modified
                if (m_lastIndex != -1)
                {
                    RestoreCachedVertexAttributes(m_lastIndex);
                    m_lastIndex = -1;
                }
            }

        }
        void DrawTextHighlight(Vector3 start, Vector3 end, TMP_TextInfo m_textInfo, Color32 m_fontColor32, ref int index, Color32 highlightColor)
        {
            int underlineMaterialIndex = 0;// m_Underline.materialIndex;

            int verticesCount = index + 4;

            // Check to make sure our current mesh buffer allocations can hold these new Quads.
            if (verticesCount > m_textInfo.meshInfo[underlineMaterialIndex].vertices.Length)
            {
                // Resize Mesh Buffers
                m_textInfo.meshInfo[underlineMaterialIndex].ResizeMeshInfo(verticesCount / 4);
            }

            // UNDERLINE VERTICES FOR (3) LINE SEGMENTS
            #region HIGHLIGHT VERTICES
            Vector3[] vertices = m_textInfo.meshInfo[underlineMaterialIndex].vertices;

            // Front Part of the Underline
            vertices[index + 0] = start; // BL
            vertices[index + 1] = new Vector3(start.x, end.y, 0); // TL
            vertices[index + 2] = end; // TR
            vertices[index + 3] = new Vector3(end.x, start.y, 0); // BR
            #endregion

            // UNDERLINE UV0
            #region HANDLE UV0
            Vector2[] uvs0 = m_textInfo.meshInfo[underlineMaterialIndex].uvs0;

            int atlasWidth = m_textInfo.textComponent.font.atlasWidth;
            int atlasHeight = m_textInfo.textComponent.font.atlasHeight;
            bool isUsingAlternativeTypeface;
            TMP_Character character = TMP_FontAssetUtilities.GetCharacterFromFontAsset(0x2026, TMP_Settings.defaultFontAsset, true, m_textInfo.textComponent.fontStyle, m_textInfo.textComponent.fontWeight, out isUsingAlternativeTypeface);

            GlyphRect glyphRect = character.glyph.glyphRect;

            // Calculate UV
            //Vector2 uv0 = new Vector2(((float)glyphRect.x + glyphRect.width / 2) / atlasWidth, (glyphRect.y + (float)glyphRect.height / 2) / atlasHeight);  // bottom left

            //// UVs for the Quad
            //uvs0[0 + index] = uv0; // BL
            //uvs0[1 + index] = uv0; // TL
            //uvs0[2 + index] = uv0; // TR
            //uvs0[3 + index] = uv0; // BR
            #endregion

            // HIGHLIGHT UV2
            #region HANDLE UV2 - SDF SCALE
            //Vector2[] uvs2 = m_textInfo.meshInfo[underlineMaterialIndex].uvs2;
            //Vector2 customUV = new Vector2(0, 1);
            //uvs2[0 + index] = customUV;
            //uvs2[1 + index] = customUV;
            //uvs2[2 + index] = customUV;
            //uvs2[3 + index] = customUV;
            #endregion

            // HIGHLIGHT VERTEX COLORS
            #region
            // Alpha is the lower of the vertex color or tag color alpha used.
            highlightColor.a = m_fontColor32.a < highlightColor.a ? m_fontColor32.a : highlightColor.a;

            Color32[] colors32 = m_textInfo.meshInfo[underlineMaterialIndex].colors32;
            colors32[0 + index] = highlightColor;
            colors32[1 + index] = highlightColor;
            colors32[2 + index] = highlightColor;
            colors32[3 + index] = highlightColor;
            #endregion

            index += 4;
        }


        public float maxHoverTime = 0.8f;
        public float hoverTime = 0;
        int startWordIndex = -1;
        public void OnPointerDown(PointerEventData eventData)
        {
            isHoveringObject = true;
            startWordIndex = TMP_TextUtilities.FindIntersectingWord(m_TextMeshPro, Input.mousePosition, m_Camera);
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            Reset();
        }
        protected void Reset()
        {
            isHoveringObject = false;
            hoverTime = 0;
            startWordIndex = -1;
            //m_TextMeshPro.text = m_TextMeshPro.text.Replace("<mark=#ffff00aa>","").Replace("</mark>", "");
        }
        //public void OnPointerClick(PointerEventData eventData)
        //{
        //}

        //public void OnPointerUp(PointerEventData eventData)
        //{
        //    //Debug.Log("OnPointerUp()");
        //}


        void RestoreCachedVertexAttributes(int index)
        {
            if (index == -1 || index > m_TextMeshPro.textInfo.characterCount - 1) return;

            // Get the index of the material / sub text object used by this character.
            int materialIndex = m_TextMeshPro.textInfo.characterInfo[index].materialReferenceIndex;

            // Get the index of the first vertex of the selected character.
            int vertexIndex = m_TextMeshPro.textInfo.characterInfo[index].vertexIndex;

            // Restore Vertices
            // Get a reference to the cached / original vertices.
            Vector3[] src_vertices = m_cachedMeshInfoVertexData[materialIndex].vertices;

            // Get a reference to the vertices that we need to replace.
            Vector3[] dst_vertices = m_TextMeshPro.textInfo.meshInfo[materialIndex].vertices;

            // Restore / Copy vertices from source to destination
            dst_vertices[vertexIndex + 0] = src_vertices[vertexIndex + 0];
            dst_vertices[vertexIndex + 1] = src_vertices[vertexIndex + 1];
            dst_vertices[vertexIndex + 2] = src_vertices[vertexIndex + 2];
            dst_vertices[vertexIndex + 3] = src_vertices[vertexIndex + 3];

            // Restore Vertex Colors
            // Get a reference to the vertex colors we need to replace.
            Color32[] dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;

            // Get a reference to the cached / original vertex colors.
            Color32[] src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;

            // Copy the vertex colors from source to destination.
            dst_colors[vertexIndex + 0] = src_colors[vertexIndex + 0];
            dst_colors[vertexIndex + 1] = src_colors[vertexIndex + 1];
            dst_colors[vertexIndex + 2] = src_colors[vertexIndex + 2];
            dst_colors[vertexIndex + 3] = src_colors[vertexIndex + 3];

            // Restore UV0S
            // UVS0
            Vector2[] src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
            Vector2[] dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
            dst_uv0s[vertexIndex + 0] = src_uv0s[vertexIndex + 0];
            dst_uv0s[vertexIndex + 1] = src_uv0s[vertexIndex + 1];
            dst_uv0s[vertexIndex + 2] = src_uv0s[vertexIndex + 2];
            dst_uv0s[vertexIndex + 3] = src_uv0s[vertexIndex + 3];

            // UVS2
            Vector2[] src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
            Vector2[] dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
            dst_uv2s[vertexIndex + 0] = src_uv2s[vertexIndex + 0];
            dst_uv2s[vertexIndex + 1] = src_uv2s[vertexIndex + 1];
            dst_uv2s[vertexIndex + 2] = src_uv2s[vertexIndex + 2];
            dst_uv2s[vertexIndex + 3] = src_uv2s[vertexIndex + 3];


            // Restore last vertex attribute as we swapped it as well
            int lastIndex = (src_vertices.Length / 4 - 1) * 4;

            // Vertices
            dst_vertices[lastIndex + 0] = src_vertices[lastIndex + 0];
            dst_vertices[lastIndex + 1] = src_vertices[lastIndex + 1];
            dst_vertices[lastIndex + 2] = src_vertices[lastIndex + 2];
            dst_vertices[lastIndex + 3] = src_vertices[lastIndex + 3];

            // Vertex Colors
            src_colors = m_cachedMeshInfoVertexData[materialIndex].colors32;
            dst_colors = m_TextMeshPro.textInfo.meshInfo[materialIndex].colors32;
            dst_colors[lastIndex + 0] = src_colors[lastIndex + 0];
            dst_colors[lastIndex + 1] = src_colors[lastIndex + 1];
            dst_colors[lastIndex + 2] = src_colors[lastIndex + 2];
            dst_colors[lastIndex + 3] = src_colors[lastIndex + 3];

            // UVS0
            src_uv0s = m_cachedMeshInfoVertexData[materialIndex].uvs0;
            dst_uv0s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs0;
            dst_uv0s[lastIndex + 0] = src_uv0s[lastIndex + 0];
            dst_uv0s[lastIndex + 1] = src_uv0s[lastIndex + 1];
            dst_uv0s[lastIndex + 2] = src_uv0s[lastIndex + 2];
            dst_uv0s[lastIndex + 3] = src_uv0s[lastIndex + 3];

            // UVS2
            src_uv2s = m_cachedMeshInfoVertexData[materialIndex].uvs2;
            dst_uv2s = m_TextMeshPro.textInfo.meshInfo[materialIndex].uvs2;
            dst_uv2s[lastIndex + 0] = src_uv2s[lastIndex + 0];
            dst_uv2s[lastIndex + 1] = src_uv2s[lastIndex + 1];
            dst_uv2s[lastIndex + 2] = src_uv2s[lastIndex + 2];
            dst_uv2s[lastIndex + 3] = src_uv2s[lastIndex + 3];

            // Need to update the appropriate 
            m_TextMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.All);
        }
    }
}
