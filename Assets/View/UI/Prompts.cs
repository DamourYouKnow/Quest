﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Quest.UI {
    public abstract class Prompt : MonoBehaviour {
        protected GameObject panel;
        protected Text messageText;
        protected List<PromptField> fields;
        protected List<PromptButton> buttons;
        protected int width;
        protected int verticalPadding;
        protected int horizontalPadding; 

        public Prompt(string message) {
            this.setupPanel();
            this.transformComponents();
            this.messageText.text = message;
        }

        private void setupPanel() {
            this.panel = new GameObject();
            this.panel.AddComponent<CanvasRenderer>();
            this.panel.AddComponent<RectTransform>();
            this.panel.AddComponent<Image>();
        }

        private void transformComponents() {
            
        }
    }

    public class YesNoPrompt : Prompt {
        public YesNoPrompt(string message, UnityAction acceptEvent, UnityAction declineEvent) 
        : base(message) {

        }
    }

    public abstract class PromptButton : Button { 
        public PromptButton(string text, UnityAction onClickEvent, bool selectable=true) {
            this.onClick.AddListener(onClickEvent);
        }

    }

    public abstract class PromptField : MonoBehaviour {
        protected Text fieldNameText;

        public PromptField(string name) {
            this.fieldNameText.text = name;
        }
    }

    public class SliderField : PromptField {
        private Slider slider;

        public SliderField(string name) : base(name) {

        }    
    }

    public class DropdownField : PromptField {
        private Dropdown dropdown;
        private List<string> options; 

        public DropdownField(string name) : base(name) {
            this.options = new List<string>();
        }

        public DropdownField(string name, List<string> options) : base(name) {
            this.options = options;
            this.dropdown.AddOptions(options);
        }

        public string Value {
            get { return this.options[this.dropdown.value]; }
        }

        public void AddOption(string option) {
            this.dropdown.AddOptions(new List<string>() {option});
            this.options.Add(option);
        }
    }
}