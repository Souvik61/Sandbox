using DG.Tweening;
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
        public GameObject boolFieldPrefab;
        public GameObject toggleFieldPrefab;


        public EditController editController;

        private ObjectBase _cachedObject;

        private Dictionary<string, UIField> _cachedFields;

        bool _isHidden;

        public bool IsShowDetails;

        public void Init(bool isShowDetails)
        {
            _cachedFields = new();
            _cachedObject = null;
            IsShowDetails = isShowDetails;
            ClearInspector();
        }

        private void Awake()
        {
            //Set button references    

            //Init();

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        /// <summary>
        /// Hide or unhide
        /// </summary>
        /// <param name="hide"></param>
        public void Hide(bool hide)
        {
            _isHidden = hide;

            if (_isHidden)
            {
                GetComponent<CanvasGroup>().interactable = false;
                GetComponent<CanvasGroup>().DOFade(0, 0.3f);
            }
            else
            {
                GetComponent<CanvasGroup>().interactable = true;
                GetComponent<CanvasGroup>().DOFade(1, 0.3f);
            }

        }

        public void UpdatePropertyValues()
        {
            if (_cachedFields.Count() == 0 || _cachedObject == null)
                return;

            // get all properties
            var props = _cachedObject.GetAllProperties();

            foreach (var item in props)
            {
                if (_cachedFields.ContainsKey(item.id))//This field is cached
                {
                    switch (item.proptype)
                    {
                        case PropertyType.STRING:
                            _cachedFields[item.id].Value = item.getter();
                            break;
                        case PropertyType.FLOAT:
                            _cachedFields[item.id].Value = item.getter();
                            break;
                        case PropertyType.COLOR:
                            _cachedFields[item.id].Value = item.getter();
                            break;
                        case PropertyType.BOOL:
                            _cachedFields[item.id].Value = item.getter();
                            break;
                        default:
                            break;
                    }
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
                _cachedObject = null;
                ClearInspector();
            }
            else if (obj == _cachedObject)
            {

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
                if (IsShowDetails == false)
                {
                    if (IsPropertyInDetails(item))
                        continue;
                }

                switch (item.proptype)
                {
                    case PropertyType.STRING:
                        AddTextField(item.id, item.name, item.getter().ToString());
                        break;
                    case PropertyType.FLOAT:
                        AddFloatField(item.id, item.name, (float)item.getter());
                        break;
                    case PropertyType.COLOR:
                        AddColorField(item.id, item.name, (Color)item.getter(), () => { OnColorButtonClicked(); });
                        break;
                    case PropertyType.BOOL:
                        AddBoolField(item.id, item.name, (bool)item.getter(), (val) => { item.setter(val); });
                        break;
                    case PropertyType.TOGGLE:
                        AddToggleField(item.id, item.name, (int)item.getter(), (val) => { item.setter((int)val); });
                        break;
                    default:
                        break;
                }
            }

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

        public void AddBoolField(string Id, string label, bool value, Action<bool> onButtonClick)
        {
            var field = Instantiate(boolFieldPrefab, propertiesContentRoot);
            field.SetActive(true);
            var ui = field.GetComponent<UIFieldBool>();
            ui.Initialize(Id, label, value, onButtonClick);
            _cachedFields[Id] = ui;
        }

        public void AddToggleField(string Id, string label, int value, Action<int> onValueChanged)
        {
            var field = Instantiate(toggleFieldPrefab, propertiesContentRoot);
            field.SetActive(true);
            var ui = field.GetComponent<UIFieldToggle>();
            ui.Initialize(Id, label, value, onValueChanged);
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
        
        /// <summary>
        /// Is this property a details property
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        bool IsPropertyInDetails(ObjectBase.PropertyItem prop)
        {
            return ((prop.id == "_posX") || (prop.id == "_posY") || (prop.id == "_rot"));
        }

    }
}