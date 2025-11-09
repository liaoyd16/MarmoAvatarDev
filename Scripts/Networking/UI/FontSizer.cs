using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FontSizer : MonoBehaviour
{
    RectTransform m_rect;
    [SerializeField] TextMeshProUGUI tmp_placehold, tmp_inputfield;

    [SerializeField] float height_div, font_div;

    int screen_w = Screen.width;
    int screen_h = Screen.height;

    [SerializeField] float original_y;

    // Start is called before the first frame update
    void Start()
    {
        screen_w = Screen.width;
        screen_h = Screen.height;
    }

    void Update()
    {
        m_rect = GetComponent<RectTransform>();
        m_rect.sizeDelta = new Vector2(
            m_rect.sizeDelta.x, screen_h / height_div);
        m_rect.position = new Vector2(
            m_rect.position.x, screen_h / height_div / 2 + original_y * screen_h / 100);

        tmp_placehold.fontSize = screen_w / font_div;
        if (tmp_inputfield != null)
        {
            tmp_inputfield.fontSize = screen_w / font_div;
        }
    }
}
