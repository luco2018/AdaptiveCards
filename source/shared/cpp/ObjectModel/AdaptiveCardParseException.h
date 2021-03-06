#pragma once

#include "pch.h"
#include "Enums.h"

namespace AdaptiveSharedNamespace {

class AdaptiveCardParseException : public std::exception
{
public:
    AdaptiveCardParseException(AdaptiveSharedNamespace::ErrorStatusCode statusCode, const std::string& message);
    ~AdaptiveCardParseException();

    virtual const char* what() const throw();
    AdaptiveSharedNamespace::ErrorStatusCode GetStatusCode() const;
    const std::string& GetReason() const;

private:
    const AdaptiveSharedNamespace::ErrorStatusCode m_statusCode;
    const std::string m_message;
};

}
