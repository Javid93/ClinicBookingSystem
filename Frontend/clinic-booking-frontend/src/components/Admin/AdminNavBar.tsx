import { Link } from 'react-router-dom';

const AdminNavBar = () => {
  return (
    <nav>
      <ul>
        <li>
          <Link to="/admin/appointments">Appointments</Link> {/* Link to AdminAppointments */}
        </li>
        {/* Add other admin links here */}
      </ul>
    </nav>
  );
};

export default AdminNavBar;
