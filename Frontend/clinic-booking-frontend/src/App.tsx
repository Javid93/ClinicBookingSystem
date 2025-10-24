import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Home from '@/pages/Home/Home';
import BookAppointment from '@/pages/BookAppointment/BookAppointment';
import SearchDoctor from '@/pages/SearchDoctor/SearchDoctor';
import About from '@/pages/About/About';
import Contact from '@/pages/Contact/Contact'

// for admin
import AdminAppointments from '@/pages/Admin/AdminAppointments';
import EditAppointment from '@/pages/Admin/EditAppointment';
import AdminLayout from '@/components/Admin/AdminLayout'

function App() {
  return (
    <Router>

      <Routes>  
        <Route path="/about" element={<About />} />
        <Route path="/contact" element={<Contact />} />
        <Route path="/home" element={<Home />} />
        <Route path="/" element={<Navigate to="/home"  replace/>} />
        <Route path='/book' element={<BookAppointment />} />
        <Route path='/book/:id' element={<BookAppointment />} />
        <Route path='/search' element={<SearchDoctor />} />
        
        
          <Route path="/admin/*" element={<AdminLayout />}>
          <Route path="appointments" element={<AdminAppointments />} />
          <Route path="appointments/edit/:id" element={<EditAppointment />} />
        </Route>


        <Route path="*" element={<div className="p-4">404 - Page Not Found</div>} />
        
      </Routes>
    </Router>
  );
}

export default App;