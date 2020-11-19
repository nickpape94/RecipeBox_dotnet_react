import React, { Fragment, useState } from 'react';
import { Link, Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { logout } from '../../actions/auth';
import { withRouter } from 'react-router-dom';
import noavatar from '../../img/noavatar.png';

const Navbar = ({ auth: { isAuthenticated, loading, user }, logout, history }) => {
	const [ hoverUserPic, setHoverUserPic ] = useState(false);

	console.log(hoverUserPic);

	const authLinks = (
		<ul>
			<li>
				<Link
					to={{
						pathname: '/posts',
						state: { fromNav: true }
					}}
				>
					Browse Recipes
				</Link>
			</li>
			<li>
				<Link to='/cuisines'>Cuisines</Link>
			</li>
			<li>
				{/* <a
					onClick={() => {
						logout();
						history.push('/');
					}}
				> */}

				{/* <i className='fas fa-sign-out-alt' />{' '} */}
				<span className='hide-sm'>
					{' '}
					{/* <img src={user && user.photoUrl !== null ? user.photoUrl : (noavatar)} />{' '} */}
					{user && user.photoUrl !== null ? (
						<div onMouseEnter={() => setHoverUserPic(true)} onMouseLeave={() => setHoverUserPic(false)}>
							<img src={user.photoUrl} />
							{hoverUserPic && (
								<div className='nav_dropdown'>
									<a>
										<i className='fas fa-portrait'>My Profile</i>
									</a>
									<a
										onClick={() => {
											logout();
											history.push('/');
											setHoverUserPic(false);
										}}
									>
										<i className='fas fa-sign-out-alt' /> Logout
									</a>
								</div>
							)}
						</div>
					) : (
						<div onMouseEnter={() => setHoverUserPic(true)} onMouseLeave={() => setHoverUserPic(false)}>
							<i className='fas fa-user-circle fa-3x' />
							{hoverUserPic && (
								<div>
									<a>
										<i className='fas fa-portrait'>My Profile</i>
									</a>
									<a
										onClick={() => {
											logout();
											history.push('/');
											setHoverUserPic(false);
										}}
									>
										<i className='fas fa-sign-out-alt' /> Logout
									</a>
								</div>
							)}
						</div>
					)}
				</span>
				{/* </a> */}
			</li>
		</ul>
	);

	const guestLinks = (
		<ul>
			<li>
				<Link to='/cuisines'>Cuisines</Link>
			</li>
			<li>
				<Link
					to={{
						pathname: '/posts',
						state: { fromNav: true }
					}}
				>
					Browse Recipes
				</Link>
			</li>
			<li>
				<Link to='/register'>Register</Link>
			</li>
			<li>
				<Link to='/login'>Login</Link>
			</li>
		</ul>
	);

	return (
		<nav className='navbar bg-dark'>
			<h1>
				<Link to='/'>
					<i className='fas fa-drumstick-bite' /> Recipe Box
				</Link>
			</h1>
			{!loading && <Fragment>{isAuthenticated ? authLinks : guestLinks}</Fragment>}
			{/* {console.log(user && user.photoUrl)} */}

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
		</nav>
	);
};

Navbar.propTypes = {
	logout: PropTypes.func.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, { logout })(withRouter(Navbar));
