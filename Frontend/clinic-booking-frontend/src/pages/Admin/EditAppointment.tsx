import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import api from '@/api/axios';
import { AppointmentDTO } from '@/types/AppointmentDTO';
import '@/pages/Admin/EditAppointment.css';

import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { parseISO } from 'date-fns';
import type { FormEvent } from 'react';

const EditAppointment = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [appointment, setAppointment] = useState<AppointmentDTO | null>(null);
  const [clinics, setClinics] = useState<any[]>([]);
  const [doctors, setDoctors] = useState<any[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const [apptRes, clinicRes, doctorRes] = await Promise.all([
          api.get(`/appointments/${id}`),
          api.get('/clinics'),
          api.get('/doctors'),
        ]);
        setAppointment(apptRes.data);
        setClinics(clinicRes.data);
        setDoctors(doctorRes.data);
      } catch (err) {
        setError('Failed to fetch data.');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [id]);

  const handleSubmit = async (e: FormEvent) => {
  e.preventDefault();
  setError(null);

  if (!appointment?.clinicId || !appointment.doctorId) {
    setError('Please select both a clinic and a doctor.');
    return;
  }

  const date = new Date(appointment.appointmentDateTime);
  const osloTimeStr = new Intl.DateTimeFormat('en-GB', {
    timeZone: 'Europe/Oslo',
    hour12: false,
    hour: '2-digit',
    minute: '2-digit',
  }).format(date);

  const [hourStr, minuteStr] = osloTimeStr.split(':');
  const osloMinutes = parseInt(hourStr) * 60 + parseInt(minuteStr);
  const duration = appointment.durationMinutes ?? 30;
  const endMinutes = osloMinutes + duration;

  const CLINIC_OPEN = 540;  // 09:00
  const CLINIC_CLOSE = 960; // 16:00

  if (osloMinutes < CLINIC_OPEN || osloMinutes >= CLINIC_CLOSE) {
    setError('⏰ Appointments can only be edited between 09:00 and 16:00 Oslo time.');
    return;
  }

  if (endMinutes > CLINIC_CLOSE) {
    const maxAllowed = CLINIC_CLOSE - osloMinutes;
    setError(`❌ Admins can only set duration up to ${maxAllowed} minutes from this time. The clinic closes at 16:00.`);

    return;
  }

  try {
    await api.put(`/appointments/${appointment.id}`, appointment);
    navigate('/admin/appointments');
  } catch (err: any) {
    setError(err.response?.data ?? 'Something went wrong.');
  }
};



  if (loading) return <div>Loading...</div>;

  return (
    <div className="edit-admin-container">
      <h2>Edit Appointment</h2>
      {error && <div className="edit-admin-error-popup">{error}</div>}


      {appointment && (
        <form onSubmit={handleSubmit} className="edit-admin-form">
          <div className="edit-admin-form-group">
            <div>
              <label>Appointment Date & Time</label>
              <DatePicker
                selected={parseISO(appointment.appointmentDateTime)}
                onChange={(date: Date | null) => {
                  if (date) {
                    setAppointment({
                      ...appointment,
                      appointmentDateTime: `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}T${String(date.getHours()).padStart(2, '0')}:${String(date.getMinutes()).padStart(2, '0')}`,
                    });
                  }
                }}
                showTimeSelect
                timeFormat="HH:mm"
                timeIntervals={30}
                dateFormat="yyyy-MM-dd HH:mm"
                timeCaption="Time"
                className="edit-admin-datepicker"
              />
            </div>

            <div>
              <label>Duration (minutes)</label>
              <input
                type="number"
                min="30"
                max="180"
                value={appointment.durationMinutes}
                onChange={(e) =>
                  setAppointment({
                    ...appointment,
                    durationMinutes: +e.target.value,
                  })
                }
                required
              />
            </div>

            <div>
              <label>Clinic</label>
              <select
                value={appointment.clinicId}
                onChange={(e) =>
                  setAppointment({
                    ...appointment,
                    clinicId: +e.target.value,
                  })
                }
                required
              >
                <option value="" disabled>-- Select Clinic --</option>
                {clinics.map((clinic) => (
                  <option key={clinic.id} value={clinic.id}>{clinic.name}</option>
                ))}
              </select>
            </div>

            <div>
              <label>Doctor</label>
              <select
                value={appointment.doctorId}
                onChange={(e) =>
                  setAppointment({
                    ...appointment,
                    doctorId: +e.target.value,
                  })
                }
                required
              >
                <option value="" disabled>-- Select Doctor --</option>
                {doctors.map((doctor) => (
                  <option key={doctor.id} value={doctor.id}>
                    {doctor.firstName} {doctor.lastName}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <button type="submit" className="edit-admin-submit-btn">Save Changes</button>
        </form>
      )}
    </div>
  );
};

export default EditAppointment;
