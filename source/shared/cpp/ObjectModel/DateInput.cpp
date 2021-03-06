#include "pch.h"
#include "DateInput.h"
#include "ParseUtil.h"

using namespace AdaptiveSharedNamespace;

DateInput::DateInput() :
    BaseInputElement(CardElementType::DateInput)
{
    PopulateKnownPropertiesSet();
}

Json::Value DateInput::SerializeToJsonValue() const
{
    Json::Value root = BaseInputElement::SerializeToJsonValue();

    if (!m_max.empty())
    {
        root[AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Max)] = m_max;
    }

    if (!m_min.empty())
    {
        root[AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Min)] = m_min;
    }

    if (!m_placeholder.empty())
    {
        root[AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Placeholder)] = m_placeholder;
    }

    if (!m_value.empty())
    {
        root[AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Value)] = m_value;
    }

    return root;
}

std::string DateInput::GetMax() const
{
    return m_max;
}

void DateInput::SetMax(const std::string &value)
{
    m_max = value;
}

std::string DateInput::GetMin() const
{
    return m_min;
}

void DateInput::SetMin(const std::string &value)
{
    m_min = value;
}

std::string DateInput::GetPlaceholder() const
{
    return m_placeholder;
}

void DateInput::SetPlaceholder(const std::string &value)
{
    m_placeholder = value;
}

std::string DateInput::GetValue() const
{
    return m_value;
}

void DateInput::SetValue(const std::string &value)
{
    m_value = value;
}

std::shared_ptr<BaseCardElement> DateInputParser::Deserialize(
    std::shared_ptr<ElementParserRegistration>,
    std::shared_ptr<ActionParserRegistration>,
    std::vector<std::shared_ptr<AdaptiveCardParseWarning>>&,
    const Json::Value& json)
{
    ParseUtil::ExpectTypeString(json, CardElementType::DateInput);

    std::shared_ptr<DateInput> dateInput = BaseInputElement::Deserialize<DateInput>(json);

    dateInput->SetMax(ParseUtil::GetString(json, AdaptiveCardSchemaKey::Max));
    dateInput->SetMin(ParseUtil::GetString(json, AdaptiveCardSchemaKey::Min));
    dateInput->SetPlaceholder(ParseUtil::GetString(json, AdaptiveCardSchemaKey::Placeholder));
    dateInput->SetValue(ParseUtil::GetString(json, AdaptiveCardSchemaKey::Value));

    return dateInput;
}

std::shared_ptr<BaseCardElement> DateInputParser::DeserializeFromString(
    std::shared_ptr<ElementParserRegistration> elementParserRegistration,
    std::shared_ptr<ActionParserRegistration> actionParserRegistration,
    std::vector<std::shared_ptr<AdaptiveCardParseWarning>>& warnings,
    const std::string& jsonString)
{
    return DateInputParser::Deserialize(elementParserRegistration, actionParserRegistration, warnings, ParseUtil::GetJsonValueFromString(jsonString));
}

void DateInput::PopulateKnownPropertiesSet() 
{
    m_knownProperties.insert(AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Max));
    m_knownProperties.insert(AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Min));
    m_knownProperties.insert(AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Value));
    m_knownProperties.insert(AdaptiveCardSchemaKeyToString(AdaptiveCardSchemaKey::Placeholder));
}
