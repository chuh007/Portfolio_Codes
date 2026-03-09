#pragma once
#include "Button.h"
enum class SOUND_CHANNEL;
class VolumeButton :
    public Button
{
public:
    void OnClick() override;

public:
    void SetVolumeValue(float val) { m_volumechangeValue = val; };
    void SetVolumeChannel(vector<SOUND_CHANNEL> _channel);
private:
    float m_volumechangeValue;
    vector<SOUND_CHANNEL> m_channels;
};

