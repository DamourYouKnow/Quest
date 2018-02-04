using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace Quest.UI {
    public abstract class Prompt : MonoBehaviour {
        protected string message;
        protected List<PromptField> fields;
        protected List<PromptButton> buttons;
        protected GameObject promptObject;

        public Prompt(string message) {
            this.message = message;
        }
    }

    public class YesNoPrompt : Prompt {
        public YesNoPrompt(string message, UnityAction acceptEvent, UnityAction declineEvent) 
        : base(message) {
            this.buttons.Add(new AcceptButton(acceptEvent));
            this.buttons.Add(new DeclineButton(declineEvent));
        }
    }

    public abstract class PromptButton : MonoBehaviour {
        protected string text;
        protected UnityAction onClickEvent;
        protected bool selectable;

        public PromptButton(string text, UnityAction onClickEvent, bool selectable=true) {
            this.text = text;
            this.selectable = selectable;
            this.onClickEvent = onClickEvent;
        }

        void OnMouseDown() {
            if (this.selectable) {
                this.onClickEvent.Invoke();
            }
        }
    }

    public class AcceptButton : PromptButton {
        public AcceptButton(UnityAction onClickEvent, bool selectable=true) 
        : base("Accept", onClickEvent, selectable) {

        }
    }

    public class DeclineButton : PromptButton {
        public DeclineButton(UnityAction onClickEvent, bool selectable = true)
        : base("Decline", onClickEvent, selectable) {

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