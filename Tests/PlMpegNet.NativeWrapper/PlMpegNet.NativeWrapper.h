#pragma once

#include <memory>

#define PL_MPEG_IMPLEMENTATION
#define PLM_NO_STDIO

#include "pl_mpeg.h"

using namespace System;

namespace PlMpegNativeWrapper {
	std::shared_ptr<plm_t> _plm;

	public ref class FrameInfo
	{

	};

	public ref class Native
	{
	public:
		static bool create_with_memory(array<unsigned char>^ bytes)
		{
			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char* ptr = (unsigned char*)p;

			auto plm = plm_create_with_memory(ptr, bytes->Length, 0);
			if (!plm)
			{
				return false;
			}
			_plm = std::shared_ptr<plm_t>(plm);

			return true;
		}

		static void set_audio_enabled(bool enabled)
		{
			plm_set_audio_enabled(_plm.get(), enabled ? 1 : 0);
		}

		static bool get_next_frame_rgb(array<unsigned char>^ bytes)
		{
			plm_frame_t* frame = plm_decode_video(_plm.get());
			if (!frame)
			{
				return false;
			}

			auto width = plm_get_width(_plm.get());

			pin_ptr<unsigned char> p = &bytes[0];
			unsigned char* ptr = (unsigned char*)p;
			plm_frame_to_rgb(frame, ptr, width * 3);

			return true;
		}
	};
}
