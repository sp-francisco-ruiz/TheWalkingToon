using System.Collections.Generic;
using UnityEngine;

namespace Game.Localization
{
    public class LocalizationManager
    {
        public static LocalizationManager _instance;
        public static LocalizationManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new LocalizationManager();
                }
                return _instance;
            }
        }

        SystemLanguage _language;
        Dictionary<string, string> _texts;

        public LocalizationManager()
        {
            _language = UnityEngine.Application.systemLanguage;
            switch(_language)
            {
                case SystemLanguage.Spanish:
                    InitSpanishLanguage();
                break;

                default:
                    InitEnglishLanguage();
                break;
            }
        }

        public string GetLocalizedString(string key)
        {
            string res;
            if(_texts.TryGetValue(key, out res))
            {
                return res;
            }
            return string.Empty;
        }

        void InitEnglishLanguage()
        {
            _texts = new Dictionary<string, string>()
            {
                {LocalizationKeys.UI_YES_KEY,"YES"},
                {LocalizationKeys.UI_NO_KEY,"NO"},

                {LocalizationKeys.TEST_YOU_SURE_KEY,"ARE YOU SURE?"},
                {LocalizationKeys.TEST_BEGIN_POPUP_TEST_KEY,"Do you want to start?"},
            };
        }

        void InitSpanishLanguage()
        {
            _texts = new Dictionary<string, string>()
            {
                {LocalizationKeys.UI_YES_KEY,"SI"},
                {LocalizationKeys.UI_NO_KEY,"NO"},

                {LocalizationKeys.TEST_YOU_SURE_KEY,"¿ESTAS SEGURO?"},
                {LocalizationKeys.TEST_BEGIN_POPUP_TEST_KEY,"¿De verdad quieres empezar?"},
            };
        }
    }
}