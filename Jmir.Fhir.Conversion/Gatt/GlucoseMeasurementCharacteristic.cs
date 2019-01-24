/*Copyright (c) 2015, Nordic Semiconductor ASA
 *
 *Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 *
 *1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 *
 *2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other 
 *materials provided with the distribution.
 *
 *3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived from this software without specific 
 *prior written permission.
 *
 *THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR
 *PURPOSE ARE DISCLAIMED. *IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF *SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, *DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
 *ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED *OF THE POSSIBILITY OF SUCH DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service.GattService
{
  using System.IO;


	public class GlucoseMeasurementValue
	{
		public short TimeOffset { get; set; }
		public ushort SequenceNumber { get; set; }
		public DateTime? BaseTime { get; set; }
		public float GlucoseConcentrationKgL { get; set; }
		public float GlucoseConcentrationMolL { get; set; }
		public GlucoseMeasureSampleType Type { get; set; }
		public GlucoseMeasureSampleLocation Location { get; set; }

		/*
		 * Flag in LSO to MSO, where LSO = Least Significant Octet and MSO = Most Significant Octet. click the link to see more details
		 * Sensor Status Annunciation
		 * https://developer.bluetooth.org/gatt/characteristics/Pages/CharacteristicViewer.aspx?u=org.bluetooth.characteristic.glucose_measurement.xml
		 */
		public bool IsDeviceBatteryLow { get; internal set; }
		public bool IsSensorMalfunction { get; internal set; }
		public bool IsSampleSizeInsufficient { get; internal set; }
		public bool IsStripInsertionError { get; internal set; }
		public bool IsStripTypeIncorrect { get; internal set; }
		public bool IsSensorResultTooHigh { get; internal set; }
		public bool IsSensorResultTooLow { get; internal set; }
		public bool IsSensorTemperatureTooHigh { get; internal set; }
		public bool IsSensorTemperatureTooLow { get; internal set; }
		public bool IsSensorReadInterrupted { get; internal set; }
		public bool IsGeneralDeviceFault { get; internal set; }
		public bool IsTimeFault { get; internal set; }
		
		public void SetSensorStatusAnnunciation(ushort statusAnnunciation)
		{
			IsDeviceBatteryLow = (statusAnnunciation & 0x0001) != 0;
			IsSensorMalfunction = (statusAnnunciation & 0x0002) != 0;
			IsSampleSizeInsufficient = (statusAnnunciation & 0x0004) != 0;
			IsStripInsertionError = (statusAnnunciation & 0x0008) != 0;
			IsStripTypeIncorrect = (statusAnnunciation & 0x0010) != 0;
			IsSensorResultTooHigh = (statusAnnunciation & 0x0020) != 0;
			IsSensorResultTooLow = (statusAnnunciation & 0x0040) != 0;
			IsSensorTemperatureTooHigh = (statusAnnunciation & 0x0080) != 0;
			IsSensorTemperatureTooLow = (statusAnnunciation & 0x0100) != 0;
			IsSensorReadInterrupted = (statusAnnunciation & 0x0200) != 0;
			IsGeneralDeviceFault = (statusAnnunciation & 0x0400) != 0;
			IsTimeFault = (statusAnnunciation & 0x0800) != 0;
		}

		public byte GetType(byte typeAndLocation) 
		{
			var type = (byte)(typeAndLocation & 0x0F);
			return type;
		}

		public byte GetLocation(byte typeAndLocation) 
		{
			var location = (byte)((typeAndLocation >> 4) & 0x0F);
			return location;
		}

		public GlucoseMeasureSampleLocation GetGlucoseMeasureSampleLocation(byte location) 
		{
			switch(location)
			{
				case 1:
					return GlucoseMeasureSampleLocation.Finger;
				case 2:
					return GlucoseMeasureSampleLocation.AlternateSiteTestAST;
				case 3:
					return GlucoseMeasureSampleLocation.Earlobe;
				case 4:
					return GlucoseMeasureSampleLocation.ControlSolution;
				case 15:
					return GlucoseMeasureSampleLocation.SampleLocationValueNotAvailable;
				default:
					throw new ArgumentException();
			}
		}

		public GlucoseMeasureSampleType GetGlucoseMeasureSampleType(byte type) 
		{
			switch (type) 
			{
				case 1:
					return GlucoseMeasureSampleType.CapillaryWholeblood;
				case 2:
					return GlucoseMeasureSampleType.CapillaryPlasma;
				case 3:
					return GlucoseMeasureSampleType.VenousWholeBlood;
				case 4:
					return GlucoseMeasureSampleType.VenousPlasma;
				case 5:
					return GlucoseMeasureSampleType.ArterialWholeBlood;
				case 6:
					return GlucoseMeasureSampleType.ArterialPlasma;
				case 7:
					return GlucoseMeasureSampleType.UndeterminedWholeBlood;
				case 8:
					return GlucoseMeasureSampleType.UndeterminedPlasma;
				case 9:
					return GlucoseMeasureSampleType.InterstitialFluid_ISF;
				case 10:
					return GlucoseMeasureSampleType.ControlSolution;
				default:
					throw new ArgumentException();
			}
		}
	}

	public enum GlucoseMeasureSampleLocation 
	{
		Finger = 1,
		AlternateSiteTestAST = 2,
		Earlobe = 3,
		ControlSolution = 4,
		SampleLocationValueNotAvailable = 15
	}

	public enum GlucoseMeasureSampleType
	{
		CapillaryWholeblood = 1,
		CapillaryPlasma = 2,
		VenousWholeBlood = 3,
		VenousPlasma = 4,
		ArterialWholeBlood = 5,
		ArterialPlasma = 6,
		UndeterminedWholeBlood = 7,
		UndeterminedPlasma = 8,
		InterstitialFluid_ISF = 9,
		ControlSolution = 10
	}
}
