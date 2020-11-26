import React, { Fragment, useState } from 'react';
import { Link, Redirect } from 'react-router-dom';
import { connect } from 'react-redux';
import PropTypes from 'prop-types';
import { logout } from '../../actions/auth';
import { resetProfilePagination } from '../../actions/user';
import { withRouter } from 'react-router-dom';
import noavatar from '../../img/noavatar.png';

const Navbar = ({ auth: { isAuthenticated, loading, user }, logout, resetProfilePagination, history }) => {
	const [ hoverUserPic, setHoverUserPic ] = useState(false);

	// console.log(hoverUserPic);

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
					<div onMouseEnter={() => setHoverUserPic(true)} onMouseLeave={() => setHoverUserPic(false)}>
						{user && user.userPhotos.length !== 0 ? (
							<img src={user.userPhotos[0].url} />
						) : (
							<i className='fas fa-user-circle fa-2x' />
						)}

						{hoverUserPic && (
							<div className='nav_dropdown'>
								<a
									onClick={() => {
										history.push(`/users/${user.id}/my-profile`);
										setHoverUserPic(false);
										resetProfilePagination();
									}}
								>
									<i className='fas fa-portrait' /> My Profile
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
		</nav>
	);
};

Navbar.propTypes = {
	logout: PropTypes.func.isRequired,
	auth: PropTypes.object.isRequired,
	resetProfilePagination: PropTypes.func.isRequired
};

const mapStateToProps = (state) => ({
	auth: state.auth
});

export default connect(mapStateToProps, { logout, resetProfilePagination })(withRouter(Navbar));
