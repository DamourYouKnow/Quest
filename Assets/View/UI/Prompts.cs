using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Quest.UI {
    public class Prompt : MonoBehaviour {
        protected Canvas canvas;
        protected GameObject panel;
        protected RectTransform panelTransform;
        protected Rect panelRect;
        protected Image panelImage;
        protected GameObject messageObj;
        protected Text messageText;
        protected List<PromptField> fields;
        protected List<PromptButton> buttons;

        public Color Color {
            get { return this.panelImage.color; }
            set { this.panelImage.color = value; }
        }

        public Canvas Canvas {
            get { return this.canvas; }
            set { this.canvas = value; }
        }

        public string Message {
            get { return this.messageText.text; }
            set { this.messageText.text = value; }
        }

        public void SetSize(int x, int y) {
            this.panelTransform.sizeDelta = new Vector2(x, y);
        }

        public void SetScale(double x, double y) {

        }

        public void Awake() {
            this.setupText();
        }

        public void Start() {
            this.setupPanel();
            this.setParents();
            this.transformComponents();
        }

        private void setParents() {
            this.messageObj.transform.SetParent(this.panelTransform, false);
            this.panel.transform.SetParent(this.canvas.transform, false);
        }

        private void setupPanel() {
            this.panel = new GameObject("PromptPanel");
            this.panel.AddComponent<CanvasRenderer>();
            this.panelTransform = this.panel.AddComponent<RectTransform>();
            this.panelRect = this.panelTransform.rect;
            this.panelImage = this.panel.AddComponent<Image>();
            this.Color = Color.red;
            this.SetSize(300, 200);
        }

        private void setupText() {
            this.messageObj = new GameObject("PromptText");
            this.messageText = messageObj.AddComponent<Text>();
            this.messageText.font = Utils.UnityFont.Arial();   
        }

        private void setupFields() {

        }

        private void setupButtons() {

        }

        private void transformComponents() {
            
        }
    }

    public class YesNoPrompt : Prompt {
       
    }

    public class PromptButton : Button { 


    }

    public class PromptField : MonoBehaviour {
        protected GameObject panel;
        protected Text fieldNameText;

    }

    public class InputField : MonoBehaviour {

    }

    public class SliderField : PromptField {
        private Slider slider;


    }

    public class DropdownField : PromptField {
        private Dropdown dropdown;
        private List<string> options; 

        public string Value {
            get { return this.options[this.dropdown.value]; }
        }

        public void AddOption(string option) {
            this.dropdown.AddOptions(new List<string>() {option});
            this.options.Add(option);
        }
    }
}