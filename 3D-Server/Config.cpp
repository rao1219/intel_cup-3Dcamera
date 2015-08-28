#include "Config.h"
#include <stdexcept>

Config::Config()
{
	outputBuffer[0] = '\0';
	for (int i = 0; i < CONFIG_SIZE; i++)
	{
		configData[i] = 0;
	}
	for (int i = 0; i < CONBIT_SIZE; i++)
	{
		contralBit[i] = false;
	}
}

double Config::GetConfig(ConfigIndex index)
{
    if (index < 0 || index >= CONFIG_SIZE) throw std::runtime_error("IndexError.\n");
	return configData[index];
}

double*Config::GetAllConfig()
{
	return configData;
}

void Config::SetConfig(double val, Config::ConfigIndex index)
{
    if (index < 0 || index >= CONFIG_SIZE) throw std::runtime_error("IndexError.\n");
	configData[index] = val;
}

void Config::SetAllConfig(double*data)
{
	for (int i = 0; i < CONFIG_SIZE;i++)
	{
		configData[i] = data[i];
	}
}

bool Config::Enable(Config::ConfigBit bit)
{
	if (bit >= 0 && bit < CONBIT_SIZE)
	{
		contralBit[bit] = true;
		return true;
	}
	return false;
}

bool Config::Disable(Config::ConfigBit bit)
{
	if (bit >= 0 && bit < CONBIT_SIZE)
	{
		contralBit[bit] = false;
		return true;
	}
	return false;
}

bool Config::GetStatus(Config::ConfigBit bit)
{
	if (bit >= 0 && bit < CONBIT_SIZE)
	{
		return contralBit[bit];
	}
	throw std::runtime_error("There no such Contral Bit.\n");
}
