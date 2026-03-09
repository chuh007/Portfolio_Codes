#include "pch.h"
#include "VolumeButton.h"
#include "ResourceManager.h"

void VolumeButton::OnClick()
{
	for (int i = 0; i < m_channels.size(); ++i)
	{
		float volume = 0;
		GET_SINGLE(ResourceManager)->Volume(m_channels[i], volume + m_volumechangeValue);//이거는 0.1 더해줌
	}
}

void VolumeButton::SetVolumeChannel(vector<SOUND_CHANNEL> _channel)
{
	m_channels = _channel;
}
