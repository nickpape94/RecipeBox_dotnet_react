import React, { useEffect, Fragment } from 'react';
import PropTypes from 'prop-types';
import { connect } from 'react-redux';
import { Link } from 'react-router-dom';
import { getUser } from '../../actions/user';
import Spinner from '../layout/Spinner';

const UserProfile = ({ getUser, user: { user, loading }, match }) => {
	useEffect(
		() => {
			getUser(match.params.id);
		},
		[ getUser, match.params.id ]
	);

	return (
		<Fragment>
			{user === null || loading ? (
				<Spinner />
			) : (
				<Fragment>
					<Link to='/posts' className='btn btn-light my-1'>
						<i className='fas fa-arrow-circle-left' /> Back To Posts
					</Link>

					<div className='profile-grid'>
						<div className='profile-top bg-primary p-2'>
							{user.photoUrl ? (
								<img className='round-img my-1' src={user.photoUrl} alt='' />
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
						</div>
						{/* <div className='line' /> */}
						<div className='profile-posts btn btn-dark my-4'>
							<Link to={`/users/${user.id}/posts`} className='user-links'>
								Browse {user.username.split(' ')[0]}'s Recipes
							</Link>
						</div>
						<div className='profile-favourites btn btn-success'>
							<Link to={`/users/${user.id}/favourites`} className='user-links'>
								Browse {user.username.split(' ')[0]}'s Favourites
							</Link>
						</div>
					</div>
				</Fragment>
			)}
		</Fragment>
	);
};

UserProfile.propTypes = {
	getUser: PropTypes.func.isRequired,
	user: PropTypes.object.isRequired
};

const mapStateToProps = (state) => ({
	user: state.user
});

export default connect(mapStateToProps, { getUser })(UserProfile);
