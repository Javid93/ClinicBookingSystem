import React, { useState, useEffect } from 'react';
import api from '@/api/axios';
import { AppointmentDTO } from '@/types/AppointmentDTO';
import { Link } from 'react-router-dom';

const AdminAppointments = () => {
  const [appointments, setAppointments] = useState<AppointmentDTO[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const response = await api.get('/appointments');
        setAppointments(response.data);
      } catch (err) {
        setError('Failed to fetch appointments');
      } finally {
        setLoading(false);
      }
    };
    fetchAppointments();
  }, []);

  const handleDelete = async (id: number) => {
    try {
      await api.delete(`/appointments/${id}`);
      setAppointments(appointments.filter((appt) => appt.id !== id));
    } catch (err) {
      setError('Failed to delete appointment');
    }
  };

  const handleConfirm = async (id: number) => {
    try {
      const appointmentToUpdate = appointments.find(a => a.id === id);
      if (!appointmentToUpdate) return;

      const updated = { ...appointmentToUpdate, status: 'Confirmed' };
      await api.put(`/appointments/${id}`, updated);

      setAppointments(appointments.map(a => a.id === id ? updated : a));
    } catch (err) {
      setError('Failed to confirm appointment');
    }
  };
  <div className="admin-appointments-header-row">
  <strong>Patient</strong> | <strong>Doctor</strong> | <strong>Clinic</strong>
</div>

{appointments.map((appointment) => (
  <div key={appointment.id} className="admin-appointments-row">
    {/* Render appointment details and action buttons here */}
  </div>
))}


  return (
    <div>
      <h2>Manage Appointments</h2>
      {loading && <div>Loading...</div>}
      {error && <div>{error}</div>}

      <table>
        <thead>
          <tr>
            <th>Patient</th>
            <th>Doctor</th>
            <th>Clinic</th>
            <th>Time</th>
            <th>Status</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {appointments.map((appointment) => (
            <tr key={appointment.id}>
              <td>{appointment.patientId}</td>
              <td>{appointment.doctorId}</td>
              <td>{appointment.clinicId}</td>
              <td>{appointment.appointmentDateTime}</td>
              <td>{appointment.status}</td>
             
              <Link to={`/admin/appointments/edit/${appointment.id}`} className="admin-btn-edit">
                 Edit
              </Link>
              <button className="admin-btn-delete" onClick={() => handleDelete(appointment.id)}>
                Delete
              </button>
              {appointment.status !== 'Confirmed' && (
              <button className="admin-btn-confirm" onClick={() => handleConfirm(appointment.id)}>
                 Confirm
              </button>
              )}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default AdminAppointments;
