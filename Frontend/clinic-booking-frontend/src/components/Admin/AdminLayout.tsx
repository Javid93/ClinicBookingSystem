import React from 'react';
import { Outlet } from 'react-router-dom';
import AdminNavBar from './AdminNavBar';
import '@/components/Admin/AdminLayout.css'

const AdminLayout: React.FC = () => {
  return (
    <div className="admin-layout">
      <div className="admin-overlay"></div>

      <div className="admin-navbar-wrapper">
        <AdminNavBar />
      </div>

      <div className="admin-content-container">
        <Outlet />
      </div>
    </div>
  );
};

export default AdminLayout;
