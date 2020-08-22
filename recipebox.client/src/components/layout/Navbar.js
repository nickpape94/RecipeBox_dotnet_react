import React from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
	return (
		<nav className='navbar bg-dark'>
			<h1>
				<Link to='/'>
					<i className='fas fa-drumstick-bite' /> FoodieConnector
				</Link>
			</h1>

			{/* <!-- <div className="search-bar">
          <form className="input-group ">
            <input className="form-control mr-sm-2" size="25" width="50%"
              placeholder="Search recipe or cuisine..." aria-label="Search">
              <span className="input-group-btn">        
              <button className="btn btn-primary my-1 my-sm-0" type="submit" >Search</button>
            </span>  
          </form>
        </div>  --> */}

			{/* <div className="input-group">
          <input type="text" className="form-control">
          <span className="input-group-btn">
            <button className="btn btn-default" type="button">Go!</button>
          </span>
        </div> */}

			<ul>
				<li>
					<Link to='!#'>Chefs</Link>
				</li>
				<li>
					<Link to='/register'>Register</Link>
				</li>
				<li>
					<Link to='/login'>Login</Link>
				</li>
			</ul>
		</nav>
	);
};

export default Navbar;
