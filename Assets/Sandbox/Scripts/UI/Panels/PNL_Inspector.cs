using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SandboxGame
{

    public class PNL_Inspector : MonoBehaviour
    {

        [Header("UI References")]
        public TMP_Text typeText;
        [Header("Position Text")]
        public TMP_Text txtXPosition;
        public TMP_Text txtYPosition;
        [Header("Rotation Text")]
        public TMP_Text txtZRotation;
        [Header("Color")]
        public Image colorImage;
        public Button colorButton;

        public TMP_Text titleText;
        public Transform propertiesContentRoot;
        public GameObject floatFieldPrefab;
        public GameObject colorFieldPrefab;
        public GameObject textFieldPrefab;


        public EditController editController;

        private ObjectBase _cachedObject;

        private Dictionary<string, UIField> _cachedFields;

        public void Init()
        {
            _cachedFields = new();
            _cachedObject = null;
            ClearInspector();
        }

        private void Awake()
        {
            //Set button references    

            Init();

        }

        // Start is called before the first frame update
        void Start()
        {
            //colorButton.GetComponent<Button>().onClick.AddListener(OnColorButtonClicked);

        }

        public void UpdatePropertyValues()
        {
            if (_cachedFields.Count() == 0 || _cachedObject == null)
                return;

            // get all properties
            var props = _cachedObject.GetAllProperties();

            foreach (var item in props)
            {
                switch (item.proptype)
                {
                    case ObjectBase.PropertyType.STRING:
                        _cachedFields[item.id].Value = item.getter();
                        break;
                    case ObjectBase.PropertyType.FLOAT:
                        _cachedFields[item.id].Value = item.getter();
                        break;
                    case ObjectBase.PropertyType.COLOR:
                        _cachedFields[item.id].Value = item.getter();
                        break;
                    default:
                        break;
                }
            }

        }

        /// <summary>
        /// Link Inspector to show details of that object
        /// </summary>
        /// <param name="obj"></param>
        public void Link(ObjectBase obj)
        {
            if (obj == null)
            {
                ClearInspector();
            }
            else if (obj == _cachedObject)
            {
                //typeText.text = "None";
                //txtXPosition.text = Constants.TEXTNA;
                //txtYPosition.text = Constants.TEXTNA;
                //txtZRotation.text = Constants.TEXTNA;
            }
            else if (obj != _cachedObject)
            {
                // clear previous properties
                ClearInspector();

                PopulateInspector(obj);

                _cachedObject = obj;
            }

        }

        public void PopulateInspector(ObjectBase obj)
        {
            SetTitle(obj.name);

            // get all properties
            var props = obj.GetAllProperties();

            foreach (var item in props)
            {
                switch (item.proptype)
                {
                    case ObjectBase.PropertyType.STRING:
                        AddTextField(item.id, item.name, item.getter().ToString());
                        break;
                    case ObjectBase.PropertyType.FLOAT:
                        AddFloatField(item.id, item.name, (float)item.getter());
                        break;
                    case ObjectBase.PropertyType.COLOR:
                        AddColorField(item.id, item.name, (Color)item.getter(), () => { OnColorButtonClicked(); });
                        break;
                    default:
                        break;
                }
            }

            //AddTextField("Type", ObjectName, value => ObjectName = value);
            //inspector.AddColorField("Color", Color, value => Color = value);
            //inspector.AddFloatField("Width", Width, value => Width = value);
            //inspector.AddFloatField("Height", Height, value => Height = value);


        }

        public void ClearInspector()
        {
            _cachedFields.Clear();
            SetTitle("");
            foreach (Transform child in propertiesContentRoot)
                Destroy(child.gameObject);
        }

        public void AddFloatField(string Id, string label, float value)
        {
            var field = Instantiate(floatFieldPrefab, propertiesContentRoot);
            field.SetActive(true);
            var ui = field.GetComponent<UIFieldFloat>();
            ui.Initialize(Id, label, value);
            _cachedFields[Id] = ui;
        }

        public void AddTextField(string Id, string label, string value)
        {
            var field = Instantiate(textFieldPrefab, propertiesContentRoot);
            field.SetActive(true);
            var ui = field.GetComponent<UIFieldString>();
            ui.Initialize(Id,label, value);
            _cachedFields[Id] = ui;
        }

        public void AddColorField(string Id,string label, Color value, Action onButtonClick)
        {
            var field = Instantiate(colorFieldPrefab, propertiesContentRoot);
            field.SetActive(true);
            var ui = field.GetComponent<UIFieldColor>();
            ui.Initialize(Id,label, value, onButtonClick);
            _cachedFields[Id] = ui;
        }

        //------------------
        //Set values
        //------------------

        /// <summary>
        /// Set the name of the object im displaying
        /// </summary>
        /// <param name="title"></param>
        void SetTitle(string title)
        {
            titleText.text = title;
        }

        //------------------------------
        //Button Events
        //------------------------------

        private void OnColorButtonClicked()
        {
            editController.OnColorPickButtonClicked();
        }

        //----------------------
        //Helpers
        //----------------------


    }
}