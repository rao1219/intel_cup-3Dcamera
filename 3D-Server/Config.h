#pragma once

class Config
{
public:
	enum ConfigIndex{
		CONFIG_SHOULDER_MIN, CONFIG_SHOULDER_MAX, CONFIG_HEAD_ANGLE_A, CONFIG_HEAD_ANGLE_B_MIN, CONFIG_HEAD_ANGLE_B_MAX
		, CONFIG_ABDOMEN_ANGLE, CONFIG_ASIDE_HEAD_ANGLE_MIN, CONFIG_ASIDE_HEAD_ANGLE_MAX, CONFIG_DELAY, CONFIG_SIZE
	};
	enum ConfigBit{CONBIT_ERROR,CONBIT_PLOT,CONBIT_TIME,CONBIT_GRAPH,CONBIT_ASIDE_BODY_TEST,CONBIT_SIZE};
private:
	char outputBuffer[256];
	double configData[CONFIG_SIZE];
	bool contralBit[CONBIT_SIZE];
public:
	Config();
	double GetConfig(ConfigIndex index);
	double*GetAllConfig();
	void SetConfig(double val, ConfigIndex index);
	void SetAllConfig(double*data);
	bool Enable(ConfigBit bit);
	bool Disable(ConfigBit bit);
	bool GetStatus(ConfigBit bit);
};
