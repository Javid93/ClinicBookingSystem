import React, { useState, useEffect } from 'react';
import type { ChangeEvent, FormEvent } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import api from '@/api/axios';
import type { ClinicDTO } from '@/types/ClinicDTO';
import type { DoctorDTO } from '@/types/DoctorDTO';
import type { PatientDTO } from '@/types/PatientDTO';
import type { AppointmentDTO } from '@/types/AppointmentDTO';
import './BookAppointment.css';
import Layout from '../../components/Layout';

import DatePicker, {registerLocale} from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';

import { format } from 'date-fns';
//Norwegian Bokmål
import { nb } from 'date-fns/locale/nb';
import { enUS } from 'date-fns/locale/en-US';
import { enGB } from 'date-fns/locale/en-GB';
import { match } from 'assert';
import { resolve } from 'path';

registerLocale('nb', nb);
registerLocale('en-US', enUS);
registerLocale('en-GB', enGB);



interface BookingForm {
  firstName: string;
  lastName: string;
  email: string;
  birthDate: string;
  gender: string;
  taxNumber: string;
  religion: string;
  appointmentDateTime: string; 
  durationMinutes: number |undefined;
  clinicId: string;
  doctorId: string;
}


const BookAppointment: React.FC = () => {
  const { id } = useParams<{ id?: string }>();
  const appointmentId = id ? parseInt(id, 10) : undefined;
  const navigate = useNavigate();
  const maxDateToday = new Date().toISOString().split('T')[0];

    const browserLocale = 
      ['nb', 'no', 'nn', 'nb-NO', 'no-NO'].includes(navigator.language)
      ? 'nb'
      : navigator.language === 'en-US'
      ? 'en-US'
      : 'en-GB';

  const [form, setForm] = useState<BookingForm>({
    firstName: '',
    lastName: '',
    email: '',
    birthDate: '',
    gender: '',
    taxNumber: '',
    religion: '',
    appointmentDateTime: '',
    durationMinutes: undefined,
    clinicId: '',
    doctorId: '',
  });


  const [patientId, setPatientId] = useState<number | undefined>(undefined);
  const [clinics, setClinics] = useState<ClinicDTO[]>([]);
  const [doctors, setDoctors] = useState<DoctorDTO[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [bookedTimes, setBookedTimes] = useState<Date[]>([]);


  useEffect(() => {
    const fetchData = async () => {
      try {
        await new Promise(resolve => setTimeout(resolve, 400));
        const [clinicRes, doctorRes] = await Promise.all([
          api.get<ClinicDTO[]>('/clinics'),
          api.get<DoctorDTO[]>('/doctors'),
        ]);
        setClinics(clinicRes.data);
        setDoctors(doctorRes.data);
      } catch (err) {
        setError('Failed to load initial data.');
      } finally {
        setLoading(false);
      }
    };
    fetchData();
  }, []);



  useEffect(() => {
    if (!appointmentId) return;

    api.get<AppointmentDTO>(`/appointments/${appointmentId}`)
      .then(res => {
        const dto = res.data;
        setForm(f => ({
          ...f,
          appointmentDateTime: dto.appointmentDateTime,
          durationMinutes: dto.durationMinutes,
          clinicId: String(dto.clinicId),
          doctorId: String(dto.doctorId),
        }));
        setPatientId(dto.patientId);
        return api.get<PatientDTO>(`/patients/${dto.patientId}`);
      })
      .then(res => {
        const p = res.data;
        setForm(f => ({
          ...f,
          firstName: p.firstName,
          lastName: p.lastName,
          email: p.email,
          birthDate: p.birthDate,
          gender: p.gender ?? '',
          taxNumber: p.taxNumber ?? '',
          religion: p.religion ?? '',
        }));
      })
      .finally(() => setLoading(false));

  }, [appointmentId]);




  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const res = await api.get<AppointmentDTO[]>('/appointments');
        const slots: Date[] = [];

        const oslo = Intl.DateTimeFormat().resolvedOptions().timeZone || 'Europe/Oslo';

        res.data
          .filter(a => a.doctorId === Number(form.doctorId))
          .forEach(a => {
            const utcTime = new Date(a.appointmentDateTime);
            const localStart = new Date(utcTime.toLocaleString("en-US", { timeZone: oslo}));
            localStart.setSeconds(0, 0);
            
            const duration = a.durationMinutes || 30;
            // how many 30-min slots it span
            const intervalCount = Math.ceil(duration / 30);

            for (let i = 0; i < intervalCount; i++) {
              const slot = new Date(localStart.getTime() + i * 30 * 60 * 1000)
              slots.push(slot);
            }
          });
            

          //Update the state
          setBookedTimes(slots);
      } catch (err) {
        console.error('Failed to fetch booked times:', err);
      }
    };

    if (form.doctorId) {
      fetchAppointments();
    }
    // Trigger fetch whenever doctor changes
  }, [form.doctorId]);




  const handleChange = (e: ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setForm(prev => ({
      ...prev,
      [name]: name === 'durationMinutes' ? Number(value) : value,
    }));
  };

  const handleSubmit = async (e: FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!form.clinicId || !form.doctorId) {
      setError('Please select both a clinic and a doctor.');
      return;
    }


    // Allow only characters and not numbers
    const nameFields = ['firstName', 'lastName', 'gender', 'religion'];
    for (const field of nameFields) {
      const value = form[field as keyof BookingForm]?.toString().trim() || "";
      if(/\d/.test(value)) {
        setError(`${field.charAt(0).toUpperCase() + field.slice(1)} cannot contain numbers.`);
        return;
      }
    }

    //Validate taxNumber
    if(form.taxNumber && !/^[A-Za-zÆØÅæøå0-9]+$/.test(form.taxNumber)) {
      setError('Tax number can only contain letters and numbers.');
      return;
    }

    //Validate email format
    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if(!emailPattern.test(form.email)) {
      setError('Please enter a valid email address.');
      return;
    }

    

    //Validate birthDate, must be in the past
    const birthDate = new Date(form.birthDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    if(birthDate >= today) {
      setError('Birthdate must be in the past.');
      return;
    }

    const clinicIdNum = Number(form.clinicId);
    const doctorIdNum = Number(form.doctorId);

    //Get full Oslo time from UTC
    const appointmentDateTime = new Date(form.appointmentDateTime);

    const osloTimeStr = new Intl.DateTimeFormat('en-GB', {
      timeZone: 'Europe/Oslo',
      hour12: false,
      hour: '2-digit', 
      minute: '2-digit',
    }).format(appointmentDateTime);

    const [hourStr, minuteStr] = osloTimeStr.split(':');
    const osloMinutes = parseInt(hourStr) * 60 + parseInt(minuteStr);

    //Allow appointment from 09:00 to before 16:00. (i.e., 540 to 959 minutes)
    if (osloMinutes < 540 || osloMinutes  >= 960) {
      setError('Appointments can only booked between 09:00 and 16:00 Oslo time.');
      return;
    }

    const endMinutes = osloMinutes + (form.durationMinutes ?? 30);
    if (endMinutes > 960) {
      
      const maxAllowed = 960 - osloMinutes;
      setError(`You can only book up to ${maxAllowed} minutes from this since the clinic closes at 16:00.`);
      return;
    }



    try {
      let pid = patientId;
      if (!pid) {
        const { data: newPatient } = await api.post<PatientDTO>('/patients', {
          firstName: form.firstName,
          lastName: form.lastName,
          email: form.email,
          birthDate: form.birthDate,
          gender: form.gender,
          taxNumber: form.taxNumber,
          religion: form.religion,
        });
        pid = newPatient.id;
        setPatientId(pid);
      }

      const appointmentPayLoad: AppointmentDTO = {
        id: appointmentId ?? 0,
        appointmentDateTime: form.appointmentDateTime,
        durationMinutes: form.durationMinutes ?? 30,
        patientId: pid!,
        clinicId: clinicIdNum,
        doctorId: doctorIdNum,
        status: 'pending',
        
      };

      if (!appointmentId) {
        await api.post('/appointments', appointmentPayLoad);
        setSuccess('Appointment booked successfully!');
      } else {
        await api.put(`/appointments/${appointmentId}`, appointmentPayLoad);
        setSuccess('Appointment updated successfully!');
      }
      setTimeout(() => {
        navigate('/book');
        window.location.reload();
      }, 10000);
      
    } catch (err: any ) {
      setError(err.response?.data ?? 'Something went wrong.');
    }

  };
if (loading) {
  return (
    <Layout>
      <div className="book-appointment-container">
        <div className="loader"></div>
      </div>
    </Layout>
  );
}

return (
  <Layout>
    
    <div className="book-appointment-container">
      <h2>{appointmentId ? 'Edit Appointment' : 'Book an appointment'}</h2>

      {error ? (
        <div className="error-wrapper">
        <div className="error-msg">{error}</div>
      </div>
      ) : null}
      
      {success ? (
        <div className="success-wrapper">
        <div className="success-msg">{success}</div>
        </div>
      ) : null}
      


      <form onSubmit={handleSubmit}>
        {/* Patient Fields */}
        {['firstName', 'lastName', 'email', 'birthDate', 'gender', 'taxNumber', 'religion'].map(field => (
          <div key={field} className="form-group">
            <label>
              {field.replace(/([A-ZÆØÅ])/g, ' $1').replace(/^./, c => c.toUpperCase())}
            </label>

            {field === 'birthDate' ? (
              <DatePicker 
                locale={browserLocale}
                selected={form.birthDate ? new Date(form.birthDate) : null}
                onChange={(date: Date | null) => 
                  setForm(prev => ({
                    ...prev,
                    birthDate: date ? format(date, 'yyyy-MM-dd') : ''
                  }))
                }
                
                //Explicitly set the date format to dd.MM.yyyy
                dateFormat="dd.MM.yyyy"
                //Don't allow selecting future dates
                maxDate={new Date()}
                placeholderText={form.birthDate ? format(new Date(form.birthDate), 'dd.MM.yyyy', { locale: nb }) : '14.05.1990'}
                className="custom-datepicker"
                showMonthDropdown
                showYearDropdown
                dropdownMode="select"
                required
                
                />

              ) : field === 'taxNumber' ? (
                //Tax number input Field
                <input 
                name="taxNumber"
                type="text"
                value={form.taxNumber}
                onChange={handleChange}
                required
                placeholder="123456789"
                pattern={field === 'taxNumber' ? '[A-Za-ZÆØÅæøå0-9]+' : undefined}
                title={field === 'taxNumber' ? 'Tax number can only contain letters and numbers' : ''}
                onInvalid={(e) => {
                  (e.target as HTMLInputElement).setCustomValidity('Please enter a valid tax number.');
                }}
                onInput={(e) => {
                  (e.target as HTMLInputElement).setCustomValidity('');
                }}
                />

              ) : field === 'religion' ? (
                //Religion
                <input
                  name="religion"
                  type="text"
                  value={form.religion}
                  onChange={handleChange}
                  required
                  placeholder="none"
                  onInvalid={(e) => {
                    (e.target as HTMLInputElement).setCustomValidity('Please enter your religion.');
                  }}
                  onInput={(e) => {
                    (e.target as HTMLInputElement).setCustomValidity('');
                  }}
                  />
              ) : (

            <input
              name={field}
              type="text"
              placeholder={
                field === 'firstName' ? 'John' :
                field === 'lastName' ? 'Doe' :
                field === 'email' ? 'john.doe@example.com' : 
                field === 'birthDate' ? '' :
                field === 'gender' ? 'Male' :
                ''
              }
              value={(form as any)[field]}
              onChange={handleChange}
              required={field !== 'taxNumber' && field !== 'religion'}
                
              

              onInvalid={(e) => {
                const input = e.target as HTMLInputElement;
                if(field === 'firstName') input.setCustomValidity('Please enter your first name');
                else if (field === 'lastName') input.setCustomValidity('Please enter your last name');
                else if (field === 'email') input.setCustomValidity('Please enter your email');
                else if (field === 'gender') input.setCustomValidity('Please enter your gender');
              }}
              onInput={(e) => {
                (e.target as HTMLInputElement).setCustomValidity('');
              }}
            />
            )}
          </div>
        ))}


        {/* Appointment Fields */}
        <div className="form-group">
          <label>Clinic</label>
          <select name="clinicId" value={form.clinicId} onChange={handleChange} required>
            <option value="" disabled hidden>-- Select Clinic --</option>
            {clinics.map(c => <option key={c.id} value={c.id}>{c.name}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label>Doctor</label>
          <select name="doctorId" value={form.doctorId} onChange={handleChange} required>
            <option value="" disabled hidden>-- Select Doctor --</option>
            {doctors.map(d => <option key={d.id} value={d.id}>{d.firstName} {d.lastName}</option>)}
          </select>
        </div>

        <div className="form-group">
          <label>Appointment Date & Time</label>
          <DatePicker
            locale={browserLocale}
            selected={form.appointmentDateTime ? new Date(form.appointmentDateTime) : null}

            onChange={(date: Date | null) => 
              setForm(prev => ({
                ...prev,
                appointmentDateTime: date ? format(date, "yyyy-MM-dd'T'HH:mm") : '',

              }))
            }
            showTimeSelect
            timeFormat="HH:mm"
            timeIntervals={30}
            dateFormat="dd.MM.yyyy HH:mm"
            timeCaption="Time"
            className="custom-datepicker"
            minDate={new Date()}
            placeholderText={
              form.appointmentDateTime
              ? format(new Date(form.appointmentDateTime), 'dd.MM.yyyy HH:mm', {
                locale: browserLocale === 'nb' ? nb : browserLocale === 'en-US' ? enUS : enGB
              })
              : format(new Date(), 'dd.MM.yyyy HH:mm', {
                locale: browserLocale === 'nb' ? nb : browserLocale === 'en-US' ? enUS : enGB
              })
            }
            
            timeClassName={(time: Date) => {
              const match = bookedTimes.some(
                booked => 
                booked.getHours() === time.getHours() && 
                booked.getMinutes() === time.getMinutes() &&
                booked.getDate() === time.getDate() &&
                booked.getMonth() === time.getMonth() && 
                booked.getFullYear() === time.getFullYear()
              );
              return match ? 'react-datepicker__time-list-item--booked-slot' : '';
            }}

              
            dayClassName={(date) => {
              const match = bookedTimes.some(booked => 
                booked.getFullYear() === date.getFullYear() && 
                booked.getMonth() === date.getMonth() && 
                booked.getDate() === date.getDate()
              );
              return match ? 'booked-day' : ''
            }}  

            popperPlacement="bottom-start"
            portalId='root-portal'
            required
          />
        </div>



        <div className="form-group">
          <label>Duration (minutes)</label>
          <input
            name="durationMinutes"
            type="number"
            min={30}
            max={180}
            /* only allow 30-minutes intervals */
            step={30} 
            value={form.durationMinutes ?? ''}
            onChange={handleChange}
            required
            placeholder="30"
            onInvalid={(e) => {
              const input = e.target as HTMLInputElement;
              if(!input.value) {
                input.setCustomValidity('Please enter the duration in minutes.');
              } else if (parseInt(input.value) < 30) {
                input.setCustomValidity('Duration must be at least 30 minutes.');
              } else if (parseInt(input.value) > 180) {
                input.setCustomValidity('Duration cannot exceed 180 minutes..');
              } else if (parseInt(input.value) % 30 !== 0) {
                input.setCustomValidity('Duration must be in 30-minutes steps (e.g., 30, 60, 90...).');
              }
            }}
            onInput={(e) => {
              (e.target as HTMLInputElement).setCustomValidity('');
            }}
          />
        </div>


        <button type="submit" className="submit-btn">
          {appointmentId ? 'Update Appointment' : 'Book Appointment'}
        </button>

        
      </form>
    </div>
  </Layout>
);

};

export default BookAppointment;