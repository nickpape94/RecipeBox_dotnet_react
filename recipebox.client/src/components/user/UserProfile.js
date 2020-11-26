import React, { useEffect, useState, Fragment } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Link, Redirect } from 'react-router-dom';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';
import Moment from 'react-moment';
import moment from 'moment';

const UserProfile = ({ getUser, user: { user, loading }, auth: { user: authUser }, match, history, location }) => {
	const [ userLoading, setUserLoading ] = useState(false);

	useEffect(
		() => {
			getUser(match.params.id, setUserLoading);
		},
		[ getUser, match.params.id ]
	);

	const todaysDate = new Date().toISOString().slice(0, 10).replace(/-/g, '');

	if (userLoading) {
		return <Spinner />;
	}

	// console.log(typeof match.params.id, authUser !== null && typeof authUser.id);

	if (authUser !== null && authUser.id.toString() === match.params.id) {
		return <Redirect to={`/users/${match.params.id}/my-profile`} />;
	}

	return (
		<Fragment>
			{user === null || loading ? (
				<Spinner />
			) : (
				<Fragment>
					{/* <Link to='/posts' className='btn btn-light my-1'>
						<i className='fas fa-arrow-circle-left' /> Back To Posts
					</Link> */}
					{location.state !== undefined && location.state.fromPost ? (
						<button className='btn btn-light my-1' onClick={() => history.goBack()}>
							<i className='fas fa-arrow-circle-left' /> Back
						</button>
					) : (
						<button className='btn btn-light my-1' onClick={() => history.push('/posts')}>
							<i className='fas fa-arrow-circle-left' /> Back To Posts
						</button>
					)}

					<div className='profile-grid'>
						<div className='profile-top bg-primary p-2'>
							{user && user.userPhotos.length !== 0 ? (
								<img className='round-img my-1' src={user.userPhotos[0].url} alt='' />
							) : (
								<img
									className='round-img my-1'
									src='https://kansai-resilience-forum.jp/wp-content/uploads/2019/02/IAFOR-Blank-Avatar-Image-1.jpg'
									alt='no avatar'
								/>
							)}

							<h1 className='large'>{user.username}</h1>
							{/* <p className='lead'>Chef at Gordon Ramsays</p> */}
						</div>

						<div className='profile-about bg-light p-2'>
							<h2 className='text-primary'>About</h2>
							<p>
								Lorem ipsum dolor sit amet consectetur, adipisicing elit. Sed doloremque nesciunt,
								repellendus nostrum deleniti recusandae nobis neque modi perspiciatis similique?
							</p>
							<div className='line' />
							<ul>
								<li>
									Member Since: <Moment format='DD/MM/YYYY'>{user.created}</Moment>{' '}
								</li>
								{todaysDate === moment(user.lastActive).format('YYYYMMDD') ? (
									<li>Last Online: Today </li>
								) : (
									<li>
										Last Online: <Moment format='DD/MM/YYYY'>{user.lastActive}</Moment>{' '}
									</li>
								)}

								{/* <li>{user.posts.length} Recipes Submitted</li> */}
							</ul>
							{/* <div>
								<small>Member Since {user.created}</small>
							</div>
							<div>
								<small>Last Online {user.lastActive}</small>
							</div> */}
						</div>

						<Link to={`/users/${user.id}/posts`} className='profile-posts btn btn-dark my-4'>
							Browse {user.username.split(' ')[0]}'s Recipes
						</Link>
						<Link to={`/users/${user.id}/favourites`} className='profile-favourites btn btn-success'>
							Browse {user.username.split(' ')[0]}'s Favourites
						</Link>
					</div>
				</Fragment>
			)}
		</Fragment>
	);
};

UserProfile.propTypes = {
	getUser: PropTypes.func.isRequired,
	user: PropTypes.object.isRequired,
	auth: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	user: state.user,
	auth: state.auth
});

export default connect(mapStateToProps, { getUser })(UserProfile);
