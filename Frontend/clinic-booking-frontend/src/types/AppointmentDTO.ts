export interface AppointmentDTO {
    id: number;
    appointmentDateTime: string; //yyyy-MM-ddTHH:mm
    durationMinutes: number;
    patientId: number;
    doctorId: number;
    clinicId: number;
    status: string;
} 